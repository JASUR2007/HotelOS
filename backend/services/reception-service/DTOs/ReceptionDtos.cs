namespace HotelOS.ReceptionService.DTOs;

/// <summary>
/// Request DTO for checking in a guest.
/// </summary>
/// <param name="GuestName">Guest's full name <example>John Doe</example></param>
/// <param name="Email">Guest's email address <example>john@example.com</example></param>
/// <param name="Adults">Number of adult guests <example>2</example></param>
/// <param name="Kids">Number of children <example>1</example></param>
/// <param name="CheckInDate">Check-in date <example>2026-06-01</example></param>
/// <param name="CheckOutDate">Check-out date <example>2026-06-05</example></param>
public sealed record CheckInRequestDto(string GuestName, string Email, int Adults, int Kids, DateOnly CheckInDate, DateOnly CheckOutDate);

/// <summary>
/// Request DTO for checking out a guest.
/// </summary>
/// <param name="BookingId">Booking identifier <example>1</example></param>
/// <param name="Notes">Check-out notes <example>Room keys returned, minibar charged</example></param>
public sealed record CheckOutRequestDto(int BookingId, string Notes);

/// <summary>
/// Request DTO for placing a hold on a reservation.
/// </summary>
/// <param name="RoomId">Room identifier <example>1</example></param>
/// <param name="RoomNumber">Room number string <example>101</example></param>
/// <param name="CheckInDate">Planned check-in date <example>2026-06-01</example></param>
/// <param name="CheckOutDate">Planned check-out date <example>2026-06-05</example></param>
/// <param name="GuestsCount">Number of guests <example>2</example></param>
public sealed record HoldReservationDto(int RoomId, string RoomNumber, DateOnly CheckInDate, DateOnly CheckOutDate, int GuestsCount);

/// <summary>
/// Response DTO containing booking summary information.
/// </summary>
/// <param name="BookingId">Booking identifier <example>1</example></param>
/// <param name="RoomId">Room identifier <example>1</example></param>
/// <param name="Status">Current booking status <example>Active</example></param>
/// <param name="GuestName">Name of the guest <example>John Doe</example></param>
public sealed record BookingResponseDto(int BookingId, int RoomId, string Status, string GuestName);

/// <summary>
/// Response DTO with basic guest information.
/// </summary>
/// <param name="GuestId">Guest identifier <example>1</example></param>
/// <param name="FullName">Guest's full name <example>John Doe</example></param>
/// <param name="Email">Guest's email address <example>john@example.com</example></param>
public sealed record GuestDto(int GuestId, string FullName, string Email);

/// <summary>
/// Request DTO for creating a new guest.
/// </summary>
/// <param name="FullName">Guest's full name <example>John Doe</example></param>
/// <param name="Email">Guest's email address <example>john@example.com</example></param>
public sealed record CreateGuestDto(string FullName, string Email);

/// <summary>
/// Response DTO with full booking record details.
/// </summary>
/// <param name="Id">Booking identifier <example>1</example></param>
/// <param name="GuestName">Guest's full name <example>John Doe</example></param>
/// <param name="RoomNumber">Assigned room number <example>101</example></param>
/// <param name="Status">Booking status <example>CheckedIn</example></param>
/// <param name="CheckInDate">Check-in date string <example>2026-06-01</example></param>
/// <param name="CheckOutDate">Check-out date string <example>2026-06-05</example></param>
/// <param name="Total">Total price <example>250.00</example></param>
public sealed record BookingRecordDto(int Id, string GuestName, string RoomNumber, string Status, string CheckInDate, string CheckOutDate, decimal Total);

/// <summary>
/// Response DTO with guest record and active stay information.
/// </summary>
/// <param name="Id">Guest identifier <example>1</example></param>
/// <param name="Name">Guest's full name <example>John Doe</example></param>
/// <param name="RoomNumber">Assigned room number <example>101</example></param>
/// <param name="CheckIn">Check-in date string <example>2026-06-01</example></param>
/// <param name="CheckOut">Check-out date string <example>2026-06-05</example></param>
/// <param name="Balance">Outstanding balance <example>250.00</example></param>
public sealed record GuestRecordDto(int Id, string Name, string RoomNumber, string CheckIn, string CheckOut, decimal Balance);

/// <summary>
/// Response DTO for guest's own reservation lookup.
/// </summary>
/// <param name="Id">Booking identifier <example>1</example></param>
/// <param name="RoomNumber">Room number string <example>101</example></param>
/// <param name="Status">Reservation status <example>Active</example></param>
/// <param name="CheckInDate">Check-in date string <example>2026-06-01</example></param>
/// <param name="CheckOutDate">Check-out date string <example>2026-06-05</example></param>
/// <param name="Nights">Number of nights <example>4</example></param>
/// <param name="Total">Total price <example>250.00</example></param>
public sealed record MyReservationDto(int Id, string RoomNumber, string Status, string CheckInDate, string CheckOutDate, int Nights, decimal Total);

/// <summary>
/// Response DTO for expired (stale) bookings.
/// </summary>
/// <param name="Id">Booking identifier <example>1</example></param>
/// <param name="RoomId">Room identifier <example>1</example></param>
/// <param name="GuestName">Guest's full name <example>John Doe</example></param>
/// <param name="RoomNumber">Room number string <example>101</example></param>
/// <param name="ReservedAt">Timestamp when the booking was made <example>2026-06-01T14:30:00Z</example></param>
public sealed record ExpiredBookingDto(int Id, int RoomId, string GuestName, string RoomNumber, DateTimeOffset ReservedAt);

/// <summary>
/// Request DTO for patching a booking's status.
/// </summary>
/// <param name="Status">New status value <example>CheckedIn</example></param>
public sealed record PatchBookingStatusDto(string Status);

/// <summary>
/// Request DTO for updating an existing booking.
/// </summary>
/// <param name="GuestName">Updated guest name <example>John Doe</example></param>
/// <param name="RoomNumber">Updated room number <example>101</example></param>
/// <param name="Status">Updated booking status <example>Active</example></param>
/// <param name="CheckInDate">Updated check-in date <example>2026-06-01</example></param>
/// <param name="CheckOutDate">Updated check-out date <example>2026-06-05</example></param>
public sealed record UpdateBookingDto(string GuestName, string RoomNumber, string Status, DateOnly CheckInDate, DateOnly CheckOutDate);

/// <summary>
/// Request DTO for updating an existing guest.
/// </summary>
/// <param name="FullName">Updated guest name <example>John Doe</example></param>
/// <param name="Email">Updated email address <example>john@example.com</example></param>
public sealed record UpdateGuestDto(string FullName, string Email);
