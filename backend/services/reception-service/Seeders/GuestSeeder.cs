using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Seeders;

public static class GuestSeeder
{
    public static async Task SeedAsync(ReceptionDbContext context)
    {
        if (await context.Guests.AnyAsync())
        {
            return;
        }

        context.Guests.AddRange(
            new Guest { Id = 1, FullName = "Amelia Stone", Email = "amelia.stone@hotelos.dev" },
            new Guest { Id = 2, FullName = "Daniel Reed", Email = "daniel.reed@hotelos.dev" }
        );

        await context.SaveChangesAsync();
    }
}