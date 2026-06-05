using HotelOS.MaintenanceService.Data;
using HotelOS.MaintenanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.MaintenanceService.Seeders;

public static class MaintenanceSeeder
{
    public static async Task SeedAsync(MaintenanceDbContext context)
    {
        if (await context.MaintenanceIssues.AnyAsync())
        {
            return;
        }

        context.MaintenanceIssues.AddRange(
            new MaintenanceIssue { Id = 1, RoomNumber = "302", Title = "AC not cooling", Priority = "Critical", Status = "Assigned" },
            new MaintenanceIssue { Id = 2, RoomNumber = "118", Title = "Bathroom light flickering", Priority = "Medium", Status = "Queued" }
        );

        await context.SaveChangesAsync();
    }
}