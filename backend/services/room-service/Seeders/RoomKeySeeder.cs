using HotelOS.RoomService.Data;
using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelOS.RoomService.Seeders;

public static class RoomKeySeeder
{
    public static async Task SeedAsync(RoomDbContext context, ILogger? logger = null)
    {
        if (await context.Set<RoomKey>().AnyAsync())
        {
            logger?.LogInformation("RoomKeySeeder: Room keys already exist, skipping");
            return;
        }

        logger?.LogInformation("RoomKeySeeder: Loading rooms");
        var rooms = await context.Rooms.ToListAsync();
        logger?.LogInformation("RoomKeySeeder: Found {Count} rooms", rooms.Count);

        var keys = rooms.Select(room => new RoomKey
        {
            RoomId = room.Id,
            RoomNumber = room.RoomNumber,
            KeyType = "Room",
            Status = "Available",
            CreatedAt = DateTimeOffset.UtcNow
        }).ToList();

        context.Set<RoomKey>().AddRange(keys);
        logger?.LogInformation("RoomKeySeeder: Saving {Count} keys", keys.Count);
        await context.SaveChangesAsync();
        logger?.LogInformation("RoomKeySeeder: Done");
    }
}
