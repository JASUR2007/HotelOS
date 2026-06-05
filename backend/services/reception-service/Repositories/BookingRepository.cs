using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Repositories;

public sealed class BookingRepository(ReceptionDbContext context) : IBookingRepository
{
    public async Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            var hasOverlap = await context.Bookings.AnyAsync(existing =>
                existing.RoomId == booking.RoomId &&
                existing.Status != "CheckedOut" &&
                existing.CheckInDate < booking.CheckOutDate &&
                booking.CheckInDate < existing.CheckOutDate,
                cancellationToken);

            if (hasOverlap)
            {
                throw new InvalidOperationException($"Room {booking.RoomId} is already booked for the requested period.");
            }

            context.Bookings.Add(booking);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return booking;
        });
    }

    public Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default)
        => context.Bookings.FirstOrDefaultAsync(booking => booking.Id == bookingId, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Bookings.AsNoTracking().OrderByDescending(booking => booking.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetExpiredAsync(string status, int olderThanMinutes, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-olderThanMinutes);
        return await context.Bookings
            .Where(b => b.Status == status && b.CreatedAt < cutoff)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task PatchStatusAsync(int bookingId, string status, CancellationToken cancellationToken = default)
    {
        var booking = await context.Bookings.FindAsync([bookingId], cancellationToken)
            ?? throw new InvalidOperationException("Booking not found");
        booking.Status = status;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        context.Entry(booking).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await context.Bookings.FindAsync([bookingId], cancellationToken)
            ?? throw new InvalidOperationException("Booking not found");
        context.Bookings.Remove(booking);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}