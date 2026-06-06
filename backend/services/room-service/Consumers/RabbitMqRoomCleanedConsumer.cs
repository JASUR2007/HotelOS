using System.Text;
using System.Text.Json;
using HotelOS.RoomService.Data;
using HotelOS.RoomService.Models;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HotelOS.RoomService.Consumers;

public sealed class RabbitMqRoomCleanedConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqRoomCleanedConsumer> logger) : BackgroundService
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
                var queue = channel.QueueDeclare("hotelos.room.cleaned.events", durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queue.QueueName, RabbitMqQueues.TopicExchange, RabbitMqRoutingKeys.RoomCleaned);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (_, args) =>
                {
                    try
                    {
                        var body = args.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                        if (data is null || !data.TryGetValue("RoomId", out var roomIdEl))
                        {
                            channel.BasicAck(args.DeliveryTag, multiple: false);
                            return;
                        }

                        var roomId = roomIdEl.GetInt32();

                        using var scope = scopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<RoomDbContext>();

                        var room = await context.Rooms.FindAsync([roomId], stoppingToken);
                        if (room is not null)
                        {
                            room.Status = "Available";
                            await context.SaveChangesAsync(stoppingToken);
                            logger.LogInformation("Room {RoomId} set to Available after cleaning", roomId);
                        }

                        channel.BasicAck(args.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to process room.cleaned event");
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
                logger.LogWarning(exception, "Room-cleaned consumer retry in {DelaySeconds}s (attempt {Attempt})", (int)delay.TotalSeconds, retryAttempt);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
