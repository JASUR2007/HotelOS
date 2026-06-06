using Microsoft.EntityFrameworkCore;
using HotelOS.HousekeepingService.Models;

namespace HotelOS.HousekeepingService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HousekeepingDbContext>();
        await context.Database.EnsureCreatedAsync();

        if (!await context.CleaningTasks.AnyAsync())
        {
            context.CleaningTasks.AddRange(
                new CleaningTask { RoomId = 101, RoomNumber = "101", Status = "Dirty", AssignedTo = "Unassigned", Priority = "Normal" },
                new CleaningTask { RoomId = 102, RoomNumber = "102", Status = "Cleaning", AssignedTo = "Maria Lopez", Priority = "High" },
                new CleaningTask { RoomId = 205, RoomNumber = "205", Status = "Clean", AssignedTo = "Maria Lopez", Priority = "Low" },
                new CleaningTask { RoomId = 302, RoomNumber = "302", Status = "Needs Inspection", AssignedTo = "Unassigned", Priority = "Normal" }
            );
            await context.SaveChangesAsync();
        }
    }
}
