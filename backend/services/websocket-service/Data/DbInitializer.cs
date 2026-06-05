using Microsoft.EntityFrameworkCore;

namespace HotelOS.WebsocketService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WebsocketDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}