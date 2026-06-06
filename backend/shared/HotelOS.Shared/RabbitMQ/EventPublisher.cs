using System.Text;
using System.Text.Json;
using HotelOS.Shared.Constants;
using HotelOS.Shared.Dtos;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace HotelOS.Shared.RabbitMQ;

public interface IEventPublisher
{
    void Publish(string routingKey, object @event);
}

public sealed class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly object _lock = new();
    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed;

    public RabbitMqEventPublisher(IConnectionFactory connectionFactory, ILogger<RabbitMqEventPublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public void Publish(string routingKey, object @event)
    {
        EnsureConnection();

        if (_channel is null) return;

        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: RabbitMqQueues.TopicExchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish event with routing key {RoutingKey}", routingKey);
        }
    }

    private void EnsureConnection()
    {
        if (_channel is not null) return;

        lock (_lock)
        {
            if (_channel is not null) return;

            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(RabbitMqQueues.TopicExchange, ExchangeType.Topic, durable: true, autoDelete: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to connect to RabbitMQ. Events will not be published.");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try { _channel?.Close(); _channel?.Dispose(); } catch { }
        try { _connection?.Close(); _connection?.Dispose(); } catch { }
    }
}
