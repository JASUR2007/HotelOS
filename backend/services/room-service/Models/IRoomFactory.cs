namespace HotelOS.RoomService.Models;

public interface IRoomFactory
{
    Room Create(string roomNumber, int floor);
}

public sealed class SingleRoomFactory : IRoomFactory
{
    public Room Create(string roomNumber, int floor) => new()
    {
        RoomNumber = roomNumber,
        Type = RoomType.Single,
        Status = "Available",
        PricePerNight = 120m,
        Floor = floor,
        Description = "Cozy single room ideal for solo travelers. Compact yet thoughtfully designed with a workspace, comfortable single bed, and all essential amenities.",
        GuestCapacity = 1,
        MainImage = "/images/rooms/room-2.png",
        Images = new[] { "/images/rooms/room-2.png", "/images/rooms/room-2-lg.png" }
    };
}

public sealed class DoubleRoomFactory : IRoomFactory
{
    public Room Create(string roomNumber, int floor) => new()
    {
        RoomNumber = roomNumber, Type = RoomType.Double, Status = "Available",
        PricePerNight = 220m, Floor = floor,
        Description = "Modern double room with city view. Two comfortable beds, sleek design and premium bedding for a restful night.",
        GuestCapacity = 2, MainImage = "/images/rooms/room-1.png",
        Images = new[] { "/images/rooms/room-1.png", "/images/rooms/room-1-lg.png" }
    };
}

public sealed class SuiteRoomFactory : IRoomFactory
{
    public Room Create(string roomNumber, int floor) => new()
    {
        RoomNumber = roomNumber, Type = RoomType.Suite, Status = "Available",
        PricePerNight = 350m, Floor = floor,
        Description = "Luxury suite with separate living area and premium amenities. King-size bed, private balcony, and a dedicated workspace.",
        GuestCapacity = 4, MainImage = "/images/rooms/room-4.png",
        Images = new[] { "/images/rooms/room-4.png", "/images/rooms/room-4-lg.png" }
    };
}

public sealed class AccessibleRoomFactory : IRoomFactory
{
    public Room Create(string roomNumber, int floor) => new()
    {
        RoomNumber = roomNumber, Type = RoomType.Accessible, Status = "Available",
        PricePerNight = 180m, Floor = floor,
        Description = "Wheelchair-accessible room with adapted bathroom and wide doorways. Comfort and accessibility without compromise.",
        GuestCapacity = 2, MainImage = "/images/rooms/room-3.png",
        Images = new[] { "/images/rooms/room-3.png", "/images/rooms/room-3-lg.png" }
    };
}

public static class RoomFactory
{
    public static Room Create(RoomType type, string roomNumber, int floor)
    {
        IRoomFactory factory = type switch
        {
            RoomType.Single => new SingleRoomFactory(),
            RoomType.Double => new DoubleRoomFactory(),
            RoomType.Suite => new SuiteRoomFactory(),
            RoomType.Accessible => new AccessibleRoomFactory(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown room type")
        };

        return factory.Create(roomNumber, floor);
    }
}
