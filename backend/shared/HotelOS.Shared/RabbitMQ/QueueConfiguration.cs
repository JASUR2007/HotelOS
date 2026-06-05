namespace HotelOS.Shared.RabbitMQ;

public sealed record QueueConfiguration(string QueueName, bool Durable = true, bool Exclusive = false, bool AutoDelete = false);