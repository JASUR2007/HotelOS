namespace HotelOS.PaymentService.Events;

public sealed record BookingExpiredEvent(
    int BookingId,
    string GuestName,
    string RoomNumber,
    DateTimeOffset ReservedAt,
    DateTimeOffset ExpiredAt
);
