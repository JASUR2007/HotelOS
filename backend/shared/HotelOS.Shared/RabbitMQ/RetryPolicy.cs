namespace HotelOS.Shared.RabbitMQ;

public static class RetryPolicy
{
    public static TimeSpan[] ExponentialBackoffDelays { get; } =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30)
    ];
}