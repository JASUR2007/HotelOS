namespace HotelOS.GatewayApi.Models;

public sealed class GatewayAuditLog
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? ServiceName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
