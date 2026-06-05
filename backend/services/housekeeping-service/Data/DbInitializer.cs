using Microsoft.EntityFrameworkCore;

namespace HotelOS.HousekeepingService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HousekeepingDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
