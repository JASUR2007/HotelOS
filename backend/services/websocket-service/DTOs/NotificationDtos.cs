namespace HotelOS.WebsocketService.DTOs;

public sealed record RealtimeNotificationDto(string Id, string Title, string Message, string CreatedAt, string Type, bool IsRead = false, string? TargetRole = null);