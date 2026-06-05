using HotelOS.ReceptionService.DTOs;

namespace HotelOS.ReceptionService.Services;

public interface IReceptionQueries
{
    Task<IReadOnlyList<BookingRecordDto>> GetBookingsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GuestRecordDto>> GetGuestsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MyReservationDto>> GetReservationsByEmailAsync(string? email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpiredBookingDto>> GetExpiredBookingsAsync(string status, int minutes, CancellationToken cancellationToken = default);
}
