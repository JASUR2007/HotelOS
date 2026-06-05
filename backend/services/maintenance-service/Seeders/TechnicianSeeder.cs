using HotelOS.MaintenanceService.Data;
using HotelOS.MaintenanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.MaintenanceService.Seeders;

public static class TechnicianSeeder
{
    public static async Task SeedAsync(MaintenanceDbContext context)
    {
        if (await context.TechnicianAssignments.AnyAsync())
        {
            return;
        }

        context.TechnicianAssignments.AddRange(
            new TechnicianAssignment { Id = 1, IssueId = 1, TechnicianName = "Alex Martin" },
            new TechnicianAssignment { Id = 2, IssueId = 2, TechnicianName = "Sara Khan" }
        );

        await context.SaveChangesAsync();
    }
}