namespace HotelOS.WebsocketService.DTOs;

/// <summary>Represents a real-time notification sent via WebSocket to connected clients.</summary>
/// <param name="Id">The unique identifier for the notification. <example>1</example></param>
/// <param name="Title">The display title of the notification. <example>New Booking</example></param>
/// <param name="Message">The full message content body. <example>Room 101 has been booked by John Doe</example></param>
/// <param name="CreatedAt">The ISO 8601 timestamp when the notification was created. <example>2026-06-01T10:00:00Z</example></param>
/// <param name="Type">The category of the notification. <example>Booking</example></param>
public sealed record RealtimeNotificationDto(string Id, string Title, string Message, string CreatedAt, string Type);