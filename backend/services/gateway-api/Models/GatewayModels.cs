namespace HotelOS.GatewayApi.Models;

public sealed class ServiceHealth
{
    public string Service { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CheckedAt { get; set; }
}