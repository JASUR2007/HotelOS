using HotelOS.ReceptionService.Models;

namespace HotelOS.ReceptionService.Repositories;

public interface IBookingRepository
{
    Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetExpiredAsync(string status, int olderThanMinutes, CancellationToken cancellationToken = default);
    Task PatchStatusAsync(int bookingId, string status, CancellationToken cancellationToken = default);
    Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default);
    Task DeleteAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<bool> AnyOverlapForGuestAsync(int guestId, DateOnly checkInDate, DateOnly checkOutDate, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}