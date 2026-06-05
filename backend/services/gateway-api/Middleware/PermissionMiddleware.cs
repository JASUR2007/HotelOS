using System.Security.Claims;

namespace HotelOS.GatewayApi.Middleware;

public sealed class PermissionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requiredPermission = context.GetEndpoint()?.Metadata.GetMetadata<RequiresPermissionAttribute>();
        if (requiredPermission is null)
        {
            await next(context);
            return;
        }

        var permissions = context.User.FindAll("permission").Select(claim => claim.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!permissions.Contains(requiredPermission.Permission))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Missing permission" });
            return;
        }

        await next(context);
    }
}