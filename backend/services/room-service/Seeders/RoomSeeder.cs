using HotelOS.RoomService.Data;
using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelOS.RoomService.Seeders;

public static class RoomSeeder
{
    public static async Task SeedAsync(RoomDbContext context, ILogger? logger = null)
    {
        if (await context.Rooms.AnyAsync())
        {
            logger?.LogInformation("RoomSeeder: Rooms already exist, skipping");
            return;
        }

        logger?.LogInformation("RoomSeeder: Loading amenities");
        var amenities = await context.Amenities.ToDictionaryAsync(a => a.Name);
        logger?.LogInformation("RoomSeeder: Found {Count} amenities", amenities.Count);

        var rooms = new List<(Room Room, string[] AmenityNames)>
        {
            (new Room
            {
                BranchId = 1, RoomNumber = "101", Type = RoomType.Double, Status = "Available",
                PricePerNight = 220m, Floor = 1, GuestCapacity = 2,
                MainImage = "/images/rooms/room-1.png",
                Images = new[] { "/images/rooms/room-1.png", "/images/rooms/room-1-lg.png" },
                Description = "Modern double room with city view, featuring two comfortable beds. Perfect for couples or friends traveling together. Enjoy the sleek design and premium bedding for a restful night."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath" }),

            (new Room
            {
                BranchId = 1, RoomNumber = "102", Type = RoomType.Single, Status = "Available",
                PricePerNight = 120m, Floor = 1, GuestCapacity = 1,
                MainImage = "/images/rooms/room-2.png",
                Images = new[] { "/images/rooms/room-2.png", "/images/rooms/room-2-lg.png" },
                Description = "Cozy single room ideal for solo travelers. Compact yet thoughtfully designed with a workspace, comfortable single bed, and all essential amenities."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "TV", "Bath" }),

            (new Room
            {
                BranchId = 1, RoomNumber = "103", Type = RoomType.Accessible, Status = "Available",
                PricePerNight = 180m, Floor = 1, GuestCapacity = 2,
                MainImage = "/images/rooms/room-3.png",
                Images = new[] { "/images/rooms/room-3.png", "/images/rooms/room-3-lg.png" },
                Description = "Wheelchair-accessible room with adapted bathroom and wide doorways. Designed for comfort and accessibility without compromising on style or amenities."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath", "Wheelchair Access" }),

            (new Room
            {
                BranchId = 1, RoomNumber = "201", Type = RoomType.Suite, Status = "Available",
                PricePerNight = 350m, Floor = 2, GuestCapacity = 4,
                MainImage = "/images/rooms/room-4.png",
                Images = new[] { "/images/rooms/room-4.png", "/images/rooms/room-4-lg.png" },
                Description = "Luxury suite with separate living area and premium amenities. Features a king-size bed, private balcony with panoramic views, Jacuzzi bathtub, and a dedicated workspace."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath", "Jacuzzi", "Balcony" }),

            (new Room
            {
                BranchId = 1, RoomNumber = "202", Type = RoomType.Double, Status = "Available",
                PricePerNight = 240m, Floor = 2, GuestCapacity = 2,
                MainImage = "/images/rooms/room-5.png",
                Images = new[] { "/images/rooms/room-5.png", "/images/rooms/room-5-lg.png" },
                Description = "Spacious double room with park view. Two queen-size beds with premium linens, a sitting area, and large windows that fill the room with natural light."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath" }),

            (new Room
            {
                BranchId = 2, RoomNumber = "203", Type = RoomType.Single, Status = "Available",
                PricePerNight = 130m, Floor = 2, GuestCapacity = 1,
                MainImage = "/images/rooms/room-6.png",
                Images = new[] { "/images/rooms/room-6.png", "/images/rooms/room-6-lg.png" },
                Description = "Elegant single room with garden view. A peaceful retreat with a comfortable single bed, modern bathroom, and a charming view of the hotel gardens."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "TV", "Bath" }),

            (new Room
            {
                BranchId = 2, RoomNumber = "204", Type = RoomType.Double, Status = "Available",
                PricePerNight = 260m, Floor = 2, GuestCapacity = 2,
                MainImage = "/images/rooms/room-7.png",
                Images = new[] { "/images/rooms/room-7.png", "/images/rooms/room-7-lg.png" },
                Description = "Premium double room with balcony overlooking the city skyline. Features two double beds, a private balcony, and elegant furnishings throughout."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath", "Balcony" }),

            (new Room
            {
                BranchId = 2, RoomNumber = "205", Type = RoomType.Suite, Status = "Available",
                PricePerNight = 420m, Floor = 2, GuestCapacity = 4,
                MainImage = "/images/rooms/room-8.png",
                Images = new[] { "/images/rooms/room-8.png", "/images/rooms/room-8-lg.png" },
                Description = "Presidential suite with panoramic city views. The ultimate luxury experience with a grand living room, dining area, marble bathroom with Jacuzzi, private terrace, and dedicated butler service."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath", "Jacuzzi", "Balcony", "Butler Service" }),

            (new Room
            {
                BranchId = 2, RoomNumber = "301", Type = RoomType.Suite, Status = "Available",
                PricePerNight = 380m, Floor = 3, GuestCapacity = 4,
                MainImage = "/images/rooms/room-6.png",
                Images = new[] { "/images/rooms/room-6.png", "/images/rooms/room-6-lg.png" },
                Description = "Corner suite with wrap-around windows offering stunning 270-degree views. Features a California king bed, spa-like bathroom, and elegant sitting area with a writing desk."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath", "Jacuzzi", "Balcony" }),

            (new Room
            {
                BranchId = 2, RoomNumber = "302", Type = RoomType.Accessible, Status = "Available",
                PricePerNight = 200m, Floor = 3, GuestCapacity = 2,
                MainImage = "/images/rooms/room-8.png",
                Images = new[] { "/images/rooms/room-8.png", "/images/rooms/room-8-lg.png" },
                Description = "Premium accessible room with roll-in shower and widened doorways. Designed with universal design principles, offering both comfort and full accessibility with elegant furnishings."
            },
            new[] { "WiFi", "Coffee Machine", "Air Conditioning", "Smart TV", "Mini Bar", "Bath", "Wheelchair Access", "Roll-in Shower" }),
        };

        foreach (var (room, amenityNames) in rooms)
        {
            foreach (var name in amenityNames)
            {
                if (amenities.TryGetValue(name, out var amenity))
                {
                    room.Amenities.Add(amenity);
                }
                else
                {
                    logger?.LogWarning("RoomSeeder: Amenity '{Name}' not found for room {Room}", name, room.RoomNumber);
                }
            }
            context.Rooms.Add(room);
            logger?.LogInformation("RoomSeeder: Added room {Room} with {Count} amenities", room.RoomNumber, room.Amenities.Count);
        }

        logger?.LogInformation("RoomSeeder: Saving {Count} rooms", rooms.Count);
        await context.SaveChangesAsync();
        logger?.LogInformation("RoomSeeder: Done");
    }
}
