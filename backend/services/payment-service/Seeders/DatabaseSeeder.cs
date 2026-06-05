using HotelOS.PaymentService.Data;

namespace HotelOS.PaymentService.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

        await InvoiceSeeder.SeedAsync(context);
        await PaymentSeeder.SeedAsync(context);

        if (scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            await DevelopmentSeeder.SeedAsync(context);
        }
    }
}
