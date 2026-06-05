using HotelOS.PaymentService.Data;
using HotelOS.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.PaymentService.Seeders;

public static class InvoiceSeeder
{
    public static async Task SeedAsync(PaymentDbContext context)
    {
        if (await context.Invoices.AnyAsync())
        {
            return;
        }

        context.Invoices.AddRange(
            new Invoice { Id = 1, InvoiceNumber = "INV-10021", GuestName = "Amelia Stone", RoomNumber = "101", TotalAmount = 420m, Status = "Paid" },
            new Invoice { Id = 2, InvoiceNumber = "INV-10022", GuestName = "Daniel Reed", RoomNumber = "205", TotalAmount = 750m, Status = "Open" },
            new Invoice { Id = 3, InvoiceNumber = "INV-10023", GuestName = "Sofia Andersson", RoomNumber = "302", TotalAmount = 320m, Status = "Paid" },
            new Invoice { Id = 4, InvoiceNumber = "INV-10024", GuestName = "Leo Martinez", RoomNumber = "408", TotalAmount = 580m, Status = "Overdue" },
            new Invoice { Id = 5, InvoiceNumber = "INV-10025", GuestName = "Emma Wilson", RoomNumber = "112", TotalAmount = 190m, Status = "Open" }
        );

        await context.SaveChangesAsync();
    }
}
