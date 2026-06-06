namespace HotelOS.ReceptionService.DTOs;

public sealed record CheckInRequestDto(string GuestName, string Email, int Adults, int Kids, DateOnly CheckInDate, DateOnly CheckOutDate);

public sealed record CheckOutRequestDto(int BookingId, string Notes);

public sealed record HoldReservationDto(int RoomId, string RoomNumber, DateOnly CheckInDate, DateOnly CheckOutDate, int GuestsCount);

public sealed record BookingResponseDto(int BookingId, int RoomId, string Status, string GuestName);

public sealed record GuestDto(int GuestId, string FullName, string Email);

public sealed record CreateGuestDto(string FullName, string Email);

public sealed record BookingRecordDto(int Id, string GuestName, string RoomNumber, string Status, string CheckInDate, string CheckOutDate, decimal Total);

public sealed record GuestRecordDto(int Id, string Name, string RoomNumber, string CheckIn, string CheckOut, decimal Balance);

public sealed record MyReservationDto(int Id, string RoomNumber, string Status, string CheckInDate, string CheckOutDate, int Nights, decimal Total);

public sealed record ExpiredBookingDto(int Id, int RoomId, string GuestName, string RoomNumber, DateTimeOffset ReservedAt);

public sealed record PatchBookingStatusDto(string Status);

public sealed record UpdateBookingDto(string GuestName, string RoomNumber, string Status, DateOnly CheckInDate, DateOnly CheckOutDate);

public sealed record UpdateGuestDto(string FullName, string Email);