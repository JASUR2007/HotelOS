using System.Text;
using System.Text.Json;
using HotelOS.HousekeepingService.Data;
using HotelOS.HousekeepingService.Models;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HotelOS.HousekeepingService.Consumers;

public sealed class RabbitMqHousekeepingConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqHousekeepingConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retryAttempt = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                    Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = configuration["RabbitMQ:Password"] ?? "guest",
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(RabbitMqQueues.TopicExchange, ExchangeType.Topic, durable: true, autoDelete: false);
                var queue = channel.QueueDeclare("hotelos.housekeeping.events", durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queue.QueueName, RabbitMqQueues.TopicExchange, RabbitMqRoutingKeys.GuestCheckedOut);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (_, args) =>
                {
                    try
                    {
                        var body = args.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                        if (data is null || !data.TryGetValue("RoomNumber", out var roomNumberEl))
                        {
                            channel.BasicAck(args.DeliveryTag, multiple: false);
                            return;
                        }

                        var roomNumber = roomNumberEl.GetString() ?? "Unknown";
                        var roomId = data.TryGetValue("RoomId", out var rid) ? rid.GetInt32() : 0;

                        using var scope = scopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<HousekeepingDbContext>();

                        context.CleaningTasks.Add(new CleaningTask
                        {
                            RoomId = roomId,
                            RoomNumber = roomNumber,
                            Status = "Queued",
                            AssignedTo = "Unassigned",
                            Priority = "Normal"
                        });
                        await context.SaveChangesAsync(stoppingToken);

                        logger.LogInformation("Auto-created cleaning task for Room {RoomNumber} after checkout", roomNumber);
                        channel.BasicAck(args.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to process guest.checked_out event");
                        channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                channel.BasicConsume(queue.QueueName, autoAck: false, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

                retryAttempt = 0;
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                var delay = RetryPolicy.ExponentialBackoffDelays[Math.Min(retryAttempt, RetryPolicy.ExponentialBackoffDelays.Length - 1)];
                retryAttempt++;
                logger.LogWarning(exception, "Housekeeping consumer retry in {DelaySeconds}s (attempt {Attempt})", (int)delay.TotalSeconds, retryAttempt);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
