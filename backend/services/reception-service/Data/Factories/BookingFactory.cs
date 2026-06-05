using HotelOS.ReceptionService.Models;

namespace HotelOS.ReceptionService.Data.Factories;

public static class BookingFactory
{
    public static Booking Create(int guestId, int roomId) => new()
    {
        GuestId = guestId,
        RoomId = roomId,
        CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
        CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
        Status = "Booked"
    };
}