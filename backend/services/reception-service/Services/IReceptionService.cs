using HotelOS.ReceptionService.DTOs;

namespace HotelOS.ReceptionService.Services;

public interface IReceptionService
{
    Task<BookingResponseDto> CheckInAsync(CheckInRequestDto request, CancellationToken cancellationToken = default);
    Task<BookingResponseDto> CheckOutAsync(CheckOutRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingRecordDto>> GetBookingsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GuestRecordDto>> GetGuestsAsync(CancellationToken cancellationToken = default);
    Task<GuestDto> CreateGuestAsync(CreateGuestDto request, CancellationToken cancellationToken = default);
}
