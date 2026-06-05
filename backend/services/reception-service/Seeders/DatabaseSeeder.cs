using HotelOS.ReceptionService.Data;

namespace HotelOS.ReceptionService.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReceptionDbContext>();

        await GuestSeeder.SeedAsync(context);
        await BookingSeeder.SeedAsync(context);
        await InvoiceSeeder.SeedAsync(context);

        if (scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            await DevelopmentSeeder.SeedAsync(context);
        }
    }
}