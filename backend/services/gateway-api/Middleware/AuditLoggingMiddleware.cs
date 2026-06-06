using System.Security.Claims;
using HotelOS.GatewayApi.Models;
using HotelOS.GatewayApi.Repositories;

namespace HotelOS.GatewayApi.Middleware;

public sealed class AuditLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IEventLogRepository repository, ILogger<AuditLoggingMiddleware> logger)
    {
        await next(context);

        var path = context.Request.Path.Value ?? string.Empty;
        var method = context.Request.Method;

        var isRoomMutation = path.Contains("/room/rooms", StringComparison.OrdinalIgnoreCase) && method != "GET";
        var shouldLog =
            path.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/auth/logout", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/auth/register", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/roles", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/payments", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/bookings", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/users", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/reception/check-in", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/reception/check-out", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/reception/hold", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/room/orders", StringComparison.OrdinalIgnoreCase) ||
            isRoomMutation ||
            path.Contains("/maintenance", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/housekeeping", StringComparison.OrdinalIgnoreCase);

        if (!shouldLog) return;

        var userId = context.User.FindFirst("sub")?.Value
                  ?? context.User.FindFirst("nameid")?.Value
                  ?? string.Empty;
        var userName = context.User.Identity?.Name
                    ?? context.User.FindFirst(ClaimTypes.Name)?.Value
                    ?? context.User.FindFirst("email")?.Value
                    ?? "system";
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        var entityType = DetectEntityType(path);
        var entityId = ExtractEntityId(path);
        var action = $"{method} {path}";

        try
        {
            await repository.AppendAsync(new GatewayAuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                IpAddress = ipAddress,
                ServiceName = "gateway-api",
                CreatedAt = DateTimeOffset.UtcNow
            }, context.RequestAborted);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Failed to persist audit log for {Path}", path);
        }
    }

    private static string DetectEntityType(string path) => path switch
    {
        var p when p.Contains("/auth/login") || p.Contains("/auth/logout") || p.Contains("/auth/register") => "Authentication",
        var p when p.Contains("/users") => "User",
        var p when p.Contains("/roles") => "Role",
        var p when p.Contains("/permissions") => "Permission",
        var p when p.Contains("/reception/check-in") || p.Contains("/reception/check-out") || p.Contains("/reception/hold") || p.Contains("/bookings") => "Booking",
        var p when p.Contains("/room/rooms") => "Room",
        var p when p.Contains("/room/orders") => "FoodOrder",
        var p when p.Contains("/payments") => "Payment",
        var p when p.Contains("/maintenance") => "Maintenance",
        var p when p.Contains("/housekeeping") => "Housekeeping",
        _ => "Unknown"
    };

    private static string ExtractEntityId(string path)
    {
        var segments = path.Trim('/').Split('/');
        for (int i = segments.Length - 1; i >= 0; i--)
        {
            if (int.TryParse(segments[i], out _))
                return segments[i];
        }
        return string.Empty;
    }
}