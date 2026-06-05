using Microsoft.Extensions.Configuration;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HotelOS.PaymentService.Services;

public sealed class RabbitMqPaymentConsumer(IConfiguration configuration, ILogger<RabbitMqPaymentConsumer> logger) : BackgroundService
{
    private static readonly DeadLetterConfiguration DeadLetter = new("hotelos.payments.dlx", RabbitMqQueues.EventsDlq, RabbitMqQueues.EventsDlq);

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
                RabbitMqTopology.DeclareRetryAndDeadLetterTopology(channel, RabbitMqQueues.Events, DeadLetter, TimeSpan.FromSeconds(15));

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (_, args) =>
                {
                    logger.LogInformation("Payment event received on {Queue}", RabbitMqQueues.Events);
                    channel.BasicAck(args.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(RabbitMqQueues.Events, autoAck: false, consumer: consumer);

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
                logger.LogWarning(exception, "Payment consumer retry in {DelaySeconds}s (attempt {Attempt})", (int)delay.TotalSeconds, retryAttempt);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}