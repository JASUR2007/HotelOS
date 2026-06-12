using Microsoft.EntityFrameworkCore;
using HotelOS.GatewayApi.Models;

namespace HotelOS.GatewayApi.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
        await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS audit;");
        await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS users;");

        await context.Database.MigrateAsync();

        // Add missing columns if the migration-created table is incomplete
        await context.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""UserId"" text;
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""UserName"" text;
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""EntityType"" text;
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""EntityId"" text;
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""OldValue"" text DEFAULT '';
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""NewValue"" text DEFAULT '';
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""IpAddress"" text;
            ALTER TABLE audit.audit_logs ADD COLUMN IF NOT EXISTS ""ServiceName"" text;
            ALTER TABLE audit.audit_logs ALTER COLUMN ""Action"" SET NOT NULL;
            ALTER TABLE audit.audit_logs ALTER COLUMN ""Action"" SET DEFAULT '';
            ALTER TABLE audit.audit_logs ALTER COLUMN ""CreatedAt"" SET DEFAULT now();
            CREATE INDEX IF NOT EXISTS ix_audit_logs_createdat ON audit.audit_logs (""CreatedAt"");
            CREATE INDEX IF NOT EXISTS ix_audit_logs_entitytype ON audit.audit_logs (""EntityType"");");

        // Patch first record inserted by migration which has null columns
        await context.Database.ExecuteSqlRawAsync(@"
            UPDATE audit.audit_logs SET ""UserName""='System', ""EntityType""='Startup', ""ServiceName""='gateway-api' WHERE ""Id""=1 AND ""UserName"" IS NULL;");

        // Create branches table if it doesn't exist
        await context.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS audit.branches (
                ""Id"" SERIAL PRIMARY KEY,
                ""Name"" VARCHAR(200) NOT NULL,
                ""Address"" VARCHAR(500) NOT NULL,
                ""City"" VARCHAR(100) NOT NULL,
                ""Country"" VARCHAR(100) NOT NULL,
                ""Phone"" VARCHAR(50) NOT NULL,
                ""Email"" VARCHAR(200) NOT NULL,
                ""Status"" VARCHAR(20) NOT NULL DEFAULT 'Active',
                ""CreatedAt"" TIMESTAMPTZ NOT NULL DEFAULT now()
            );
            CREATE INDEX IF NOT EXISTS ix_branches_name ON audit.branches (""Name"");");

        // Seed default branch
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO audit.branches (""Id"", ""Name"", ""Address"", ""City"", ""Country"", ""Phone"", ""Email"", ""Status"", ""CreatedAt"")
            VALUES (1, 'HotelOS Downtown', '123 Main Street', 'New York', 'USA', '+1-555-0100', 'downtown@hotelos.com', 'Active', now())
            ON CONFLICT (""Id"") DO NOTHING;");

        // Seed missing permissions beyond what the AddAuthRbac migration originally seeded
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO users.permissions (""Id"", ""Name"", ""Description"") VALUES
                (15, 'manage_workers', 'Monitor workforce status'),
                (16, 'view_maintenances', 'View maintenance requests'),
                (17, 'view_housekeeping', 'View housekeeping status'),
                (18, 'view_audit_logs', 'View audit logs'),
                (19, 'view_event_logs', 'View event logs'),
                (20, 'process_refunds', 'Process refunds'),
                (21, 'manage_branches', 'Manage hotel branches'),
                (22, 'manage_keys', 'Manage room keys and master keys')
            ON CONFLICT (""Id"") DO NOTHING;");

        if (await context.AuditLogs.CountAsync() < 3)
        {
            context.AuditLogs.AddRange(
                new GatewayAuditLog { UserName = "Super Admin", Action = "POST /api/auth/login", EntityType = "Authentication", EntityId = "", IpAddress = "::1", ServiceName = "gateway-api", CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-30) },
                new GatewayAuditLog { UserName = "Super Admin", Action = "GET /api/reception/bookings", EntityType = "Booking", EntityId = "", IpAddress = "::1", ServiceName = "gateway-api", CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-25) },
                new GatewayAuditLog { UserName = "Reception Desk", Action = "POST /api/reception/check-in", EntityType = "Booking", EntityId = "3", IpAddress = "::1", ServiceName = "gateway-api", CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-20) },
                new GatewayAuditLog { UserName = "Housekeeping Lead", Action = "POST /api/housekeeping/queue", EntityType = "Housekeeping", EntityId = "1", IpAddress = "::1", ServiceName = "gateway-api", CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-15) },
                new GatewayAuditLog { UserName = "Super Admin", Action = "POST /api/maintenance", EntityType = "Maintenance", EntityId = "1", IpAddress = "::1", ServiceName = "gateway-api", CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10) },
                new GatewayAuditLog { UserName = "Super Admin", Action = "PUT /api/room/rooms/101/status", EntityType = "Room", EntityId = "101", IpAddress = "::1", ServiceName = "gateway-api", CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5) }
            );
            await context.SaveChangesAsync();
        }
    }
}