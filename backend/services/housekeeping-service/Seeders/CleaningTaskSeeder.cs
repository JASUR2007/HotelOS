using HotelOS.HousekeepingService.Data;
using HotelOS.HousekeepingService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.HousekeepingService.Seeders;

public static class CleaningTaskSeeder
{
    public static async Task SeedAsync(HousekeepingDbContext context)
    {
        if (await context.CleaningTasks.AnyAsync())
        {
            return;
        }

        context.CleaningTasks.AddRange(
            new CleaningTask { Id = 1, RoomId = 101, RoomNumber = "101", Status = "Queued", AssignedTo = "Unassigned" },
            new CleaningTask { Id = 2, RoomId = 102, RoomNumber = "102", Status = "In Progress", AssignedTo = "Maria Lopez" },
            new CleaningTask { Id = 3, RoomId = 201, RoomNumber = "201", Status = "Queued", AssignedTo = "Unassigned" },
            new CleaningTask { Id = 4, RoomId = 205, RoomNumber = "205", Status = "Complete", AssignedTo = "Sarah Kim" },
            new CleaningTask { Id = 5, RoomId = 301, RoomNumber = "301", Status = "Queued", AssignedTo = "Unassigned" }
        );

        await context.SaveChangesAsync();
    }
}
