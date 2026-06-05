using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Seeders;

public static class InvoiceSeeder
{
    public static async Task SeedAsync(ReceptionDbContext context)
    {
        if (await context.Invoices.AnyAsync())
        {
            return;
        }

        context.Invoices.AddRange(
            new Invoice { Id = 1, BookingId = 1, Total = 420m, Currency = "USD" },
            new Invoice { Id = 2, BookingId = 2, Total = 180m, Currency = "USD" }
        );

        await context.SaveChangesAsync();
    }
}