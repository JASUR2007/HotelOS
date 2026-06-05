using HotelOS.MaintenanceService.Data;
using HotelOS.MaintenanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.MaintenanceService.Seeders;

public static class DevelopmentSeeder
{
    public static async Task SeedAsync(MaintenanceDbContext context)
    {
        if (await context.MaintenanceIssues.CountAsync() > 10)
        {
            return;
        }

        var issue = new MaintenanceIssue
        {
            RoomNumber = "407",
            Title = "Mini-bar cooling issue",
            Priority = "Low",
            Status = "Queued"
        };

        context.MaintenanceIssues.Add(issue);
        await context.SaveChangesAsync();

        context.TechnicianAssignments.Add(new TechnicianAssignment
        {
            IssueId = issue.Id,
            TechnicianName = "John Miller"
        });

        await context.SaveChangesAsync();
    }
}