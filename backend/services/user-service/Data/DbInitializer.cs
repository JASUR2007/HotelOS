using Microsoft.EntityFrameworkCore;

namespace HotelOS.UserService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS users;");
        await context.Database.MigrateAsync();
    }
}