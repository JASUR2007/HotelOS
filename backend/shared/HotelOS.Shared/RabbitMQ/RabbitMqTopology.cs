using RabbitMQ.Client;

namespace HotelOS.Shared.RabbitMQ;

public static class RabbitMqTopology
{
    public static void DeclareRetryAndDeadLetterTopology(IModel channel, string queueName, DeadLetterConfiguration deadLetter, TimeSpan retryDelay)
    {
        channel.ExchangeDeclare(deadLetter.Exchange, ExchangeType.Direct, durable: true, autoDelete: false);
        channel.QueueDeclare(deadLetter.Queue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(deadLetter.Queue, deadLetter.Exchange, deadLetter.RoutingKey);

        var retryExchange = $"{queueName}.retry.exchange";
        var retryQueue = $"{queueName}.retry";

        channel.ExchangeDeclare(retryExchange, ExchangeType.Direct, durable: true, autoDelete: false);
        channel.QueueDeclare(retryQueue, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = deadLetter.Exchange,
            ["x-dead-letter-routing-key"] = deadLetter.RoutingKey,
            ["x-message-ttl"] = (int)retryDelay.TotalMilliseconds
        });
        channel.QueueBind(retryQueue, retryExchange, retryQueue);

        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = retryExchange,
            ["x-dead-letter-routing-key"] = retryQueue
        });
    }

    public static void PublishDeadLetter(IModel channel, DeadLetterConfiguration deadLetter, ReadOnlyMemory<byte> body, IBasicProperties properties)
    {
        channel.BasicPublish(deadLetter.Exchange, deadLetter.RoutingKey, mandatory: false, basicProperties: properties, body: body);
    }
}