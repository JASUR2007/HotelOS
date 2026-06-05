using HotelOS.RoomService.Data;
using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Seeders;

public static class MenuSeeder
{
    public static async Task SeedAsync(RoomDbContext context)
    {
        if (await context.MenuItems.AnyAsync())
        {
            return;
        }

        context.MenuItems.AddRange(
            new MenuItem { Id = 1, Name = "Breakfast set", Price = 18m },
            new MenuItem { Id = 2, Name = "Club sandwich", Price = 14m },
            new MenuItem { Id = 3, Name = "Sparkling water", Price = 4m }
        );

        await context.SaveChangesAsync();
    }
}