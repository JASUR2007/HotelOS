namespace HotelOS.Shared.RabbitMQ;

public sealed record DeadLetterConfiguration(string Exchange, string Queue, string RoutingKey);