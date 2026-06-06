using Microsoft.EntityFrameworkCore;

namespace HotelOS.PaymentService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        await context.Database.EnsureCreatedAsync();

        await AddColumnIfMissingAsync(context, "payments", "InvoiceId", "integer NOT NULL DEFAULT 0");
        await AddColumnIfMissingAsync(context, "payments", "ProcessedAt", "timestamptz NOT NULL DEFAULT now()");
        await AddColumnIfMissingAsync(context, "transactions", "PaymentId", "integer NOT NULL DEFAULT 0");
        await AddColumnIfMissingAsync(context, "transactions", "CreatedAt", "timestamptz NOT NULL DEFAULT now()");
        await AddColumnIfMissingAsync(context, "payment_history", "InvoiceId", "integer NOT NULL DEFAULT 0");
        await AddColumnIfMissingAsync(context, "payment_history", "ChangedAt", "timestamptz NOT NULL DEFAULT now()");
        await AddColumnIfMissingAsync(context, "idempotent_refunds", "PaymentId", "integer NOT NULL DEFAULT 0");
        await AddColumnIfMissingAsync(context, "idempotent_refunds", "CreatedAt", "timestamptz NOT NULL DEFAULT now()");
        await AddColumnIfMissingAsync(context, "invoices", "RoomNumber", "text NOT NULL DEFAULT ''");
    }

    private static async Task AddColumnIfMissingAsync(PaymentDbContext context, string table, string column, string definition)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                $"ALTER TABLE payments.{table} ADD COLUMN IF NOT EXISTS \"{column}\" {definition}");
        }
        catch
        {
            // Column may already exist — ignore
        }
    }
}
