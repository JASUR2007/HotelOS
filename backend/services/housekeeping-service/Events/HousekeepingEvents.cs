namespace HotelOS.HousekeepingService.Events;

public sealed record CleaningQueuedEvent(int RoomId, string RoomNumber, DateTimeOffset OccurredAt);
public sealed record CleaningCompletedEvent(int RoomId, string RoomNumber, DateTimeOffset OccurredAt);