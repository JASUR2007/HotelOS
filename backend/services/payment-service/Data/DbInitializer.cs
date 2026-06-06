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
        await AddColumnIfMissingAsync(context, "invoices", "ExpiresAt", "timestamptz NOT NULL DEFAULT now()");

        // Fix missing auto-increment on primary key columns
        await FixIdentityColumnAsync(context, "payments", "Id");
        await FixIdentityColumnAsync(context, "invoices", "Id");
        await FixIdentityColumnAsync(context, "transactions", "Id");
        await FixIdentityColumnAsync(context, "payment_history", "Id");
    }

    private static async Task FixIdentityColumnAsync(PaymentDbContext context, string table, string column)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync($@"
                DO $$
                DECLARE
                    has_default boolean;
                    max_id integer;
                BEGIN
                    SELECT column_default IS NOT NULL INTO has_default
                    FROM information_schema.columns
                    WHERE table_schema='payments' AND table_name='{table}' AND column_name='{column}';

                    IF NOT has_default THEN
                        IF NOT EXISTS (
                            SELECT 1 FROM pg_class
                            WHERE relname = '{table}_{column}_seq'
                            AND relnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'payments')
                        ) THEN
                            EXECUTE 'CREATE SEQUENCE payments.{table}_{column}_seq';
                        END IF;

                        EXECUTE 'SELECT COALESCE(MAX(""{column}""), 0) FROM payments.{table}' INTO max_id;
                        PERFORM setval('payments.{table}_{column}_seq', max_id);

                        EXECUTE 'ALTER TABLE payments.{table} ALTER COLUMN ""{column}"" SET DEFAULT nextval(''payments.{table}_{column}_seq'')';
                    END IF;
                END $$;");
        }
        catch
        {
            // May already be set — ignore
        }
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
