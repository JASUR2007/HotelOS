namespace HotelOS.WebsocketService.Events;

public sealed record NotificationBroadcastRequestedEvent(string Type, string Title, string Message, DateTimeOffset OccurredAt);