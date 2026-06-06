using Microsoft.EntityFrameworkCore;

namespace HotelOS.PaymentService.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Payment service exception");

            if (exception is ArgumentException || exception is InvalidOperationException || exception is DbUpdateException)
            {
                var detail = exception.InnerException is not null
                    ? $"{exception.Message} → {exception.InnerException.Message}"
                    : exception.Message;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = detail });
                return;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Payment service error" });
        }
    }
}