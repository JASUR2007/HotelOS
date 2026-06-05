using HotelOS.RoomService.Data;
using Microsoft.Extensions.Logging;

namespace HotelOS.RoomService.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RoomDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RoomDatabaseSeeder");

        await MenuSeeder.SeedAsync(context);
        await OrderSeeder.SeedAsync(context);
        await RoomSeeder.SeedAsync(context, logger);

        if (scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            await DevelopmentSeeder.SeedAsync(context);
        }
    }
}