using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReceptionDbContext>();
        await context.Database.EnsureCreatedAsync();
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE reception.bookings ALTER COLUMN \"RowVersion\" DROP NOT NULL;"); } catch { }
        try { await context.Database.ExecuteSqlRawAsync("ALTER TABLE reception.bookings ALTER COLUMN \"RowVersion\" SET DEFAULT '';"); } catch { }
    }
}