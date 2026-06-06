using HotelOS.PaymentService.Data;
using HotelOS.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.PaymentService.Seeders;

public static class PaymentSeeder
{
    public static async Task SeedAsync(PaymentDbContext context)
    {
        if (await context.Payments.AnyAsync())
        {
            return;
        }

        context.Payments.AddRange(
            new Payment { InvoiceId = 1, Amount = 420m, Method = "Card", Status = "Completed", ProcessedAt = DateTimeOffset.UtcNow.AddDays(-2) },
            new Payment { InvoiceId = 3, Amount = 320m, Method = "Cash", Status = "Completed", ProcessedAt = DateTimeOffset.UtcNow.AddDays(-1) }
        );

        await context.SaveChangesAsync();
    }
}
