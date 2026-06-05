using HotelOS.MaintenanceService.Data;

namespace HotelOS.MaintenanceService.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MaintenanceDbContext>();

        await MaintenanceSeeder.SeedAsync(context);
        await TechnicianSeeder.SeedAsync(context);

        if (scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            await DevelopmentSeeder.SeedAsync(context);
        }
    }
}