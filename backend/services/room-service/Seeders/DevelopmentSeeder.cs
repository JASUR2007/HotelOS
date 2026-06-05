using HotelOS.RoomService.Data;
using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Seeders;

public static class DevelopmentSeeder
{
    public static async Task SeedAsync(RoomDbContext context)
    {
        if (await context.FoodOrders.CountAsync() > 10)
        {
            return;
        }

        var extraOrders = Enumerable.Range(1, 10).Select(index => new FoodOrder
        {
            RoomNumber = (300 + index).ToString(),
            GuestName = $"Guest {index}",
            Status = index % 2 == 0 ? "Preparing" : "Delivered",
            Total = 10m + index
        });

        context.FoodOrders.AddRange(extraOrders);
        await context.SaveChangesAsync();
    }
}