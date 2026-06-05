using Bogus;
using HotelOS.PaymentService.Data;
using HotelOS.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.PaymentService.Seeders;

public static class DevelopmentSeeder
{
    private static readonly string[] Methods = ["Card", "Cash", "Online", "Refund"];
    private static readonly string[] InvoiceStatuses = ["Open", "Paid", "Overdue", "Cancelled"];
    private static readonly string[] PaymentStatuses = ["Completed", "Pending", "Failed", "Refunded"];

    public static async Task SeedAsync(PaymentDbContext context)
    {
        if (await context.Invoices.CountAsync() > 10)
        {
            return;
        }

        var faker = new Faker();

        var invoices = new Faker<Invoice>()
            .RuleFor(item => item.InvoiceNumber, faker => $"INV-{10030 + faker.IndexFaker}")
            .RuleFor(item => item.GuestName, faker => faker.Name.FullName())
            .RuleFor(item => item.RoomNumber, faker => $"{faker.Random.Int(101, 520)}")
            .RuleFor(item => item.TotalAmount, faker => faker.Random.Decimal(100, 2000))
            .RuleFor(item => item.Status, faker => faker.PickRandom(InvoiceStatuses))
            .Generate(20);

        context.Invoices.AddRange(invoices);
        await context.SaveChangesAsync();

        var payments = new Faker<Payment>()
            .RuleFor(item => item.InvoiceId, faker => faker.Random.Int(1, 25))
            .RuleFor(item => item.Amount, faker => faker.Random.Decimal(50, 1500))
            .RuleFor(item => item.Method, faker => faker.PickRandom(Methods))
            .RuleFor(item => item.Status, faker => faker.PickRandom(PaymentStatuses))
            .RuleFor(item => item.ProcessedAt, faker => faker.Date.RecentOffset(30))
            .Generate(25);

        context.Payments.AddRange(payments);
        await context.SaveChangesAsync();

        var transactions = new Faker<TransactionLog>()
            .RuleFor(item => item.PaymentId, faker => faker.Random.Int(1, 30))
            .RuleFor(item => item.Message, faker => faker.Lorem.Sentence())
            .RuleFor(item => item.CreatedAt, faker => faker.Date.RecentOffset(30))
            .Generate(20);

        context.TransactionLogs.AddRange(transactions);
        await context.SaveChangesAsync();
    }
}
