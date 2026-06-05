using HotelOS.UserService.Data;

namespace HotelOS.UserService.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        await PermissionSeeder.SeedAsync(context);
        await RoleSeeder.SeedAsync(context);
        await AdminSeeder.SeedAsync(context);

        if (scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            await DevelopmentSeeder.SeedAsync(context);
        }
    }
}