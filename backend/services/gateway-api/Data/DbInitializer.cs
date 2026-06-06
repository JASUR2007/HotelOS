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

        if (!await context.AuditLogs.AnyAsync())
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