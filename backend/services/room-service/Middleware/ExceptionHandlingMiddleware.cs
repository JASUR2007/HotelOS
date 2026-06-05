using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (DbUpdateException exception)
        {
            logger.LogWarning(exception, "Room service data update exception");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new { message = "Room data could not be saved. Check for duplicate room numbers or invalid references." });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Room service exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Room service error" });
        }
    }
}
