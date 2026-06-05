using HotelOS.HousekeepingService.Data;
using HotelOS.HousekeepingService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.HousekeepingService.Seeders;

public static class RoomStatusSeeder
{
    public static async Task SeedAsync(HousekeepingDbContext context)
    {
        if (await context.RoomStatuses.AnyAsync())
        {
            return;
        }

        context.RoomStatuses.AddRange(
            new RoomStatus { Id = 1, RoomId = 101, RoomNumber = "101", Status = "Dirty" },
            new RoomStatus { Id = 2, RoomId = 102, RoomNumber = "102", Status = "Cleaning" },
            new RoomStatus { Id = 3, RoomId = 201, RoomNumber = "201", Status = "Dirty" },
            new RoomStatus { Id = 4, RoomId = 205, RoomNumber = "205", Status = "Clean" },
            new RoomStatus { Id = 5, RoomId = 301, RoomNumber = "301", Status = "Dirty" },
            new RoomStatus { Id = 6, RoomId = 103, RoomNumber = "103", Status = "Clean" },
            new RoomStatus { Id = 7, RoomId = 202, RoomNumber = "202", Status = "Inspected" },
            new RoomStatus { Id = 8, RoomId = 305, RoomNumber = "305", Status = "Dirty" }
        );

        await context.SaveChangesAsync();
    }
}
