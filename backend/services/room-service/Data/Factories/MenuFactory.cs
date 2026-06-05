using HotelOS.RoomService.Models;

namespace HotelOS.RoomService.Data.Factories;

public static class MenuFactory
{
    public static MenuItem Create(string name, decimal price) => new()
    {
        Name = name,
        Price = price
    };
}