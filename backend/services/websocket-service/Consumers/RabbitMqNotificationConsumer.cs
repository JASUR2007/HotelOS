using System.Text;
using System.Text.Json;
using HotelOS.WebsocketService.DTOs;
using HotelOS.WebsocketService.Services;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HotelOS.WebsocketService.Consumers;

public sealed class RabbitMqNotificationConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqNotificationConsumer> logger) : BackgroundService
{
    private static readonly DeadLetterConfiguration DeadLetter = new(
        "hotelos.notifications.dlx",
        RabbitMqQueues.Notifications + ".dlq",
        RabbitMqQueues.Notifications + ".dlq");

    private static readonly string[] RoutingKeys = {
        RabbitMqRoutingKeys.BookingCreated,
        RabbitMqRoutingKeys.BookingCancelled,
        RabbitMqRoutingKeys.GuestCheckedIn,
        RabbitMqRoutingKeys.GuestCheckedOut,
        RabbitMqRoutingKeys.RoomVacated,
        RabbitMqRoutingKeys.RoomCleaned,
        RabbitMqRoutingKeys.RoomOccupied,
        RabbitMqRoutingKeys.OrderCreated,
        RabbitMqRoutingKeys.OrderPreparing,
        RabbitMqRoutingKeys.OrderDelivered,
        RabbitMqRoutingKeys.MaintenanceCreated,
        RabbitMqRoutingKeys.MaintenanceAssigned,
        RabbitMqRoutingKeys.MaintenanceCompleted,
        RabbitMqRoutingKeys.PaymentCompleted,
        RabbitMqRoutingKeys.PaymentRefunded,
    };

    private static readonly Dictionary<string, string> RoutingKeyToHubEvent = new()
    {
        [RabbitMqRoutingKeys.BookingCreated] = "BookingCreated",
        [RabbitMqRoutingKeys.BookingCancelled] = "BookingCancelled",
        [RabbitMqRoutingKeys.GuestCheckedIn] = "GuestCheckedIn",
        [RabbitMqRoutingKeys.GuestCheckedOut] = "GuestCheckedOut",
        [RabbitMqRoutingKeys.RoomVacated] = "RoomVacated",
        [RabbitMqRoutingKeys.RoomCleaned] = "RoomCleaned",
        [RabbitMqRoutingKeys.RoomOccupied] = "RoomOccupied",
        [RabbitMqRoutingKeys.OrderCreated] = "OrderCreated",
        [RabbitMqRoutingKeys.OrderPreparing] = "OrderPreparing",
        [RabbitMqRoutingKeys.OrderDelivered] = "OrderDelivered",
        [RabbitMqRoutingKeys.MaintenanceCreated] = "MaintenanceCreated",
        [RabbitMqRoutingKeys.MaintenanceAssigned] = "MaintenanceAssigned",
        [RabbitMqRoutingKeys.MaintenanceCompleted] = "MaintenanceCompleted",
        [RabbitMqRoutingKeys.PaymentCompleted] = "PaymentCompleted",
        [RabbitMqRoutingKeys.PaymentRefunded] = "PaymentRefunded",
    };

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

                var exchangeName = configuration["RabbitMQ:Exchange"] ?? RabbitMqQueues.TopicExchange;
                channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

                var queueName = configuration["RabbitMQ:QueueName"] ?? RabbitMqQueues.Notifications;
                RabbitMqTopology.DeclareRetryAndDeadLetterTopology(channel, queueName, DeadLetter, TimeSpan.FromSeconds(10));

                foreach (var rk in RoutingKeys)
                {
                    channel.QueueBind(queueName, exchangeName, rk);
                }

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (_, args) =>
                {
                    try
                    {
                        var payload = Encoding.UTF8.GetString(args.Body.ToArray());
                        var routingKey = args.RoutingKey;

                        var eventName = RoutingKeyToHubEvent.TryGetValue(routingKey, out var hubEvent)
                            ? hubEvent
                            : "NotificationReceived";

                        using var scope = scopeFactory.CreateScope();
                        var broadcaster = scope.ServiceProvider.GetRequiredService<INotificationBroadcastService>();

                        var notification = BuildNotificationFromEvent(eventName, routingKey, payload);

                        await broadcaster.BroadcastAsync(
                            notification,
                            eventName,
                            JsonSerializer.Deserialize<object>(payload),
                            stoppingToken);

                        channel.BasicAck(args.DeliveryTag, multiple: false);
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Failed to process websocket notification");
                        channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                channel.BasicConsume(queueName, autoAck: false, consumer: consumer);

                logger.LogInformation("WebSocket consumer connected to RabbitMQ, listening on {Queue} bound to {Exchange}",
                    queueName, exchangeName);

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

                logger.LogWarning(
                    exception,
                    "RabbitMQ is unavailable, retrying websocket consumer in {DelaySeconds}s (attempt {Attempt})",
                    (int)delay.TotalSeconds,
                    retryAttempt);

                await Task.Delay(delay, stoppingToken);
            }
        }
    }

    private static RealtimeNotificationDto BuildNotificationFromEvent(string eventName, string routingKey, string payload)
    {
        var title = eventName switch
        {
            "BookingCreated" => "Booking Created",
            "BookingCancelled" => "Booking Cancelled",
            "GuestCheckedIn" => "Guest Checked In",
            "GuestCheckedOut" => "Guest Checked Out",
            "RoomVacated" => "Room Vacated",
            "RoomCleaned" => "Room Cleaned",
            "RoomOccupied" => "Room Occupied",
            "OrderCreated" => "Order Created",
            "OrderPreparing" => "Order Preparing",
            "OrderDelivered" => "Order Delivered",
            "MaintenanceCreated" => "Maintenance Created",
            "MaintenanceAssigned" => "Maintenance Assigned",
            "MaintenanceCompleted" => "Maintenance Completed",
            "PaymentCompleted" => "Payment Received",
            "PaymentRefunded" => "Payment Refunded",
            _ => routingKey
        };

        var type = routingKey.Split('.')[0];

        return new RealtimeNotificationDto(
            Guid.NewGuid().ToString("N"),
            title,
            payload.Length > 200 ? payload[..200] : payload,
            DateTimeOffset.UtcNow.ToString("O"),
            type);
    }
}
