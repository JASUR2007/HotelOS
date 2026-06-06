using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Services;

public sealed class ReservationExpirationService(
    IServiceScopeFactory scopeFactory,
    IHttpClientFactory httpClientFactory,
    ILogger<ReservationExpirationService> logger) : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ReleaseExpiredHoldsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error checking expired holds");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task ReleaseExpiredHoldsAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReceptionDbContext>();

        var cutoff = DateTimeOffset.UtcNow.Add(-HoldDuration);
        var expired = await dbContext.Bookings
            .Where(b => b.Status == "HELD" && b.CreatedAt < cutoff)
            .ToListAsync(stoppingToken);

        foreach (var booking in expired)
        {
            booking.Status = "EXPIRED";

            try
            {
                var roomClient = httpClientFactory.CreateClient("room-service");
                await roomClient.PatchAsync(
                    $"api/room/rooms/{booking.RoomId}/status",
                    JsonContent.Create(new { status = "Available" }),
                    stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to release room {RoomId} for expired booking {BookingId}",
                    booking.RoomId, booking.Id);
            }
        }

        if (expired.Count > 0)
        {
            await dbContext.SaveChangesAsync(stoppingToken);
            logger.LogInformation("Released {Count} expired holds", expired.Count);
        }
    }
}
