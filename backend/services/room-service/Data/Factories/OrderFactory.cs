using HotelOS.RoomService.Models;

namespace HotelOS.RoomService.Data.Factories;

public static class OrderFactory
{
    public static FoodOrder Create(string roomNumber, string guestName, string status, decimal total) => new()
    {
        RoomNumber = roomNumber,
        GuestName = guestName,
        Status = status,
        Total = total
    };
}