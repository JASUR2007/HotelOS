using System.Text;
using System.Text.Json;
using HotelOS.GatewayApi.Data;
using HotelOS.GatewayApi.Models;
using HotelOS.Shared.Audit;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HotelOS.GatewayApi.Consumers;

public sealed class AuditLogConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<AuditLogConsumer> logger) : BackgroundService
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
                var queue = channel.QueueDeclare("hotelos.audit-logs", durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queue.QueueName, RabbitMqQueues.TopicExchange, "audit.logged");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (_, args) =>
                {
                    try
                    {
                        var body = args.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var entry = JsonSerializer.Deserialize<AuditEntry>(json);

                        if (entry is null)
                        {
                            channel.BasicAck(args.DeliveryTag, multiple: false);
                            return;
                        }

                        using var scope = scopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();

                        context.AuditLogs.Add(new GatewayAuditLog
                        {
                            UserName = entry.Actor,
                            Action = entry.Action,
                            EntityType = entry.Entity,
                            EntityId = string.Empty,
                            IpAddress = string.Empty,
                            ServiceName = "backend-service",
                            CreatedAt = entry.OccurredAt
                        });
                        await context.SaveChangesAsync(stoppingToken);

                        logger.LogInformation("Persisted audit log: {Action} by {Actor}", entry.Action, entry.Actor);
                        channel.BasicAck(args.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to process audit.logged event");
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
                logger.LogWarning(exception, "AuditLog consumer retry in {DelaySeconds}s (attempt {Attempt})", (int)delay.TotalSeconds, retryAttempt);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
