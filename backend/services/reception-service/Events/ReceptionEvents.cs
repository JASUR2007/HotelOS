namespace HotelOS.ReceptionService.Events;

public sealed record ReceptionBookingCreatedEvent(int BookingId, int RoomId, string GuestName, DateTimeOffset OccurredAt);

public sealed record ReceptionBookingClosedEvent(int BookingId, int RoomId, string GuestName, DateTimeOffset OccurredAt);