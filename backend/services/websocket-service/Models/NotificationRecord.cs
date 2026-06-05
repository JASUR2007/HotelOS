namespace HotelOS.WebsocketService.Models;

public sealed class NotificationRecord
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string? TargetRole { get; set; }
}