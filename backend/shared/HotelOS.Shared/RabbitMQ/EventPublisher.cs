using System.Text;
using System.Text.Json;
using HotelOS.Shared.Constants;
using HotelOS.Shared.Dtos;
using RabbitMQ.Client;

namespace HotelOS.Shared.RabbitMQ;

public interface IEventPublisher
{
    void Publish(string routingKey, object @event);
}

public sealed class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventPublisher(IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(RabbitMqQueues.TopicExchange, ExchangeType.Topic, durable: true, autoDelete: false);
    }

    public void Publish(string routingKey, object @event)
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

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
