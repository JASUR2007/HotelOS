using Microsoft.EntityFrameworkCore;

namespace HotelOS.PaymentService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        await context.Database.EnsureCreatedAsync();

        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE payments.invoices ADD COLUMN IF NOT EXISTS \"RoomNumber\" text NOT NULL DEFAULT ''");
        }
        catch
        {
            // Column may already exist or ALTER not supported — ignore
        }
    }
}
