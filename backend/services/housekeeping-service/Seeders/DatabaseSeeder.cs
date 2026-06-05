using HotelOS.HousekeepingService.Data;

namespace HotelOS.HousekeepingService.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HousekeepingDbContext>();

        await CleaningTaskSeeder.SeedAsync(context);
        await RoomStatusSeeder.SeedAsync(context);

        if (scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            await DevelopmentSeeder.SeedAsync(context);
        }
    }
}
