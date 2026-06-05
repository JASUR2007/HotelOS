namespace HotelOS.GatewayApi.Models;

public sealed class GatewayAuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
