using HotelOS.ReceptionService.DTOs;

namespace HotelOS.ReceptionService.Services;

public interface IReceptionCommands
{
    Task<BookingResponseDto> CheckInAsync(CheckInRequestDto request, CancellationToken cancellationToken = default);
    Task<BookingResponseDto> CheckOutAsync(CheckOutRequestDto request, CancellationToken cancellationToken = default);
    Task<GuestDto> CreateGuestAsync(CreateGuestDto request, CancellationToken cancellationToken = default);
    Task<BookingResponseDto> HoldReservationAsync(HoldReservationDto request, CancellationToken cancellationToken = default);
    Task<BookingResponseDto> PatchBookingStatusAsync(int bookingId, string status, CancellationToken cancellationToken = default);
    Task<BookingResponseDto> UpdateBookingAsync(int bookingId, UpdateBookingDto request, CancellationToken cancellationToken = default);
    Task DeleteBookingAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<GuestDto> UpdateGuestAsync(int guestId, UpdateGuestDto request, CancellationToken cancellationToken = default);
    Task DeleteGuestAsync(int guestId, CancellationToken cancellationToken = default);
}
