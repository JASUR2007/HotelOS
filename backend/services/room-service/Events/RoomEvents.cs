namespace HotelOS.RoomService.Events;

public sealed record OrderCreatedEvent(int OrderId, string RoomNumber, DateTimeOffset OccurredAt);
public sealed record OrderDeliveredEvent(int OrderId, string RoomNumber, DateTimeOffset OccurredAt);