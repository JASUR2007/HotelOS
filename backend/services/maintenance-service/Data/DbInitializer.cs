using Microsoft.EntityFrameworkCore;

namespace HotelOS.MaintenanceService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MaintenanceDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}