namespace HotelOS.Shared.Events;

public sealed record GuestCheckedInEvent(
    int BookingId,
    int RoomId,
    string GuestName,
    DateTimeOffset OccurredAt);

public sealed record GuestCheckedOutEvent(
    int BookingId,
    int RoomId,
    string GuestName,
    DateTimeOffset OccurredAt);

public sealed record RoomStatusChangedEvent(
    int RoomId,
    string RoomNumber,
    string Status,
    DateTimeOffset OccurredAt);

public sealed record OrderStatusChangedEvent(
    int OrderId,
    string RoomNumber,
    string Status,
    DateTimeOffset OccurredAt);

public sealed record MaintenanceTicketUpdatedEvent(
    int TicketId,
    string Title,
    string Status,
    string Priority,
    DateTimeOffset OccurredAt);

public sealed record RealtimeNotificationEvent(
    string Type,
    string Title,
    string Message,
    DateTimeOffset OccurredAt);