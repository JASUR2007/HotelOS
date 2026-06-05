using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RoomDbContext>();
        await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS room_service;");
        await context.Database.MigrateAsync();
    }
}
