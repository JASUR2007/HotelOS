using HotelOS.RoomService.Data;

namespace HotelOS.RoomService.Seeders;

public static class OrderSeeder
{
    public static Task SeedAsync(RoomDbContext context)
    {
        // Orders come from real guest actions — no pre-seeded fake orders.
        return Task.CompletedTask;
    }
}