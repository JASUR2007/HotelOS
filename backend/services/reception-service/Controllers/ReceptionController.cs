using HotelOS.ReceptionService.DTOs;
using HotelOS.ReceptionService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.ReceptionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReceptionController(
    IReceptionQueries queries,
    IReceptionCommands commands) : ControllerBase
{
    /// <summary>
    /// Retrieves all bookings.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// GET /api/reception/bookings
    /// 
    /// Response example:
    /// 
    /// [
    ///   {
    ///     "id": 1,
    ///     "guestName": "John Doe",
    ///     "roomNumber": "101",
    ///     "status": "Active",
    ///     "checkInDate": "2026-06-01",
    ///     "checkOutDate": "2026-06-05",
    ///     "total": 250.00
    ///   }
    /// ]
    /// </remarks>
    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings(CancellationToken cancellationToken)
        => Ok(await queries.GetBookingsAsync(cancellationToken));

    /// <summary>
    /// Checks in a guest and creates a booking.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// POST /api/reception/check-in
    /// 
    /// {
    ///   "guestName": "John Doe",
    ///   "email": "john@example.com",
    ///   "adults": 2,
    ///   "kids": 1,
    ///   "checkInDate": "2026-06-01",
    ///   "checkOutDate": "2026-06-05"
    /// }
    /// 
    /// Response example:
    /// 
    /// {
    ///   "bookingId": 1,
    ///   "roomId": 1,
    ///   "status": "CheckedIn",
    ///   "guestName": "John Doe"
    /// }
    /// </remarks>
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request, CancellationToken cancellationToken)
        => Ok(await commands.CheckInAsync(request, cancellationToken));

    /// <summary>
    /// Checks out a guest and frees the room.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// POST /api/reception/check-out
    /// 
    /// {
    ///   "bookingId": 1,
    ///   "notes": "Room keys returned, minibar charged"
    /// }
    /// 
    /// Response example:
    /// 
    /// {
    ///   "bookingId": 1,
    ///   "status": "Completed"
    /// }
    /// </remarks>
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequestDto request, CancellationToken cancellationToken)
        => Ok(await commands.CheckOutAsync(request, cancellationToken));

    /// <summary>
    /// Places a hold on a room reservation without confirming.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// POST /api/reception/hold
    /// 
    /// {
    ///   "roomId": 1,
    ///   "roomNumber": "101",
    ///   "checkInDate": "2026-06-01",
    ///   "checkOutDate": "2026-06-05",
    ///   "guestsCount": 2
    /// }
    /// 
    /// Response example:
    /// 
    /// {
    ///   "bookingId": 1,
    ///   "status": "Held"
    /// }
    /// </remarks>
    [HttpPost("hold")]
    public async Task<IActionResult> HoldReservation([FromBody] HoldReservationDto request, CancellationToken cancellationToken)
        => Ok(await commands.HoldReservationAsync(request, cancellationToken));

    /// <summary>
    /// Retrieves all currently checked-in guests.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// GET /api/reception/guests
    /// 
    /// Response example:
    /// 
    /// [
    ///   {
    ///     "id": 1,
    ///     "name": "John Doe",
    ///     "roomNumber": "101",
    ///     "checkIn": "2026-06-01",
    ///     "checkOut": "2026-06-05",
    ///     "balance": 250.00
    ///   }
    /// ]
    /// </remarks>
    [HttpGet("guests")]
    public async Task<IActionResult> GetGuests(CancellationToken cancellationToken)
        => Ok(await queries.GetGuestsAsync(cancellationToken));

    /// <summary>
    /// Creates a new guest record.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// POST /api/reception/guests
    /// 
    /// {
    ///   "fullName": "John Doe",
    ///   "email": "john@example.com"
    /// }
    /// 
    /// Response example:
    /// 
    /// {
    ///   "guestId": 1,
    ///   "fullName": "John Doe",
    ///   "email": "john@example.com"
    /// }
    /// </remarks>
    [HttpPost("guests")]
    public async Task<IActionResult> CreateGuest([FromBody] CreateGuestDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateGuestAsync(request, cancellationToken));

    /// <summary>
    /// Retrieves reservations for a guest by email address.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// GET /api/reception/my-reservations?email=john@example.com
    /// 
    /// Response example:
    /// 
    /// [
    ///   {
    ///     "id": 1,
    ///     "roomNumber": "101",
    ///     "status": "Active",
    ///     "checkInDate": "2026-06-01",
    ///     "checkOutDate": "2026-06-05",
    ///     "nights": 4,
    ///     "total": 250.00
    ///   }
    /// ]
    /// </remarks>
    [HttpGet("my-reservations")]
    public async Task<IActionResult> GetMyReservations([FromQuery] string? email, CancellationToken cancellationToken)
        => Ok(await queries.GetReservationsByEmailAsync(email, cancellationToken));

    /// <summary>
    /// Retrieves bookings that have expired past a given threshold.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// GET /api/reception/bookings/expired?status=Active&amp;minutes=30
    /// 
    /// Response example:
    /// 
    /// [
    ///   {
    ///     "id": 1,
    ///     "roomId": 1,
    ///     "guestName": "John Doe",
    ///     "roomNumber": "101",
    ///     "reservedAt": "2026-06-01T14:30:00Z"
    ///   }
    /// ]
    /// </remarks>
    [HttpGet("bookings/expired")]
    public async Task<IActionResult> GetExpiredBookings([FromQuery] string status, [FromQuery] int minutes, CancellationToken cancellationToken)
        => Ok(await queries.GetExpiredBookingsAsync(status, minutes, cancellationToken));

    /// <summary>
    /// Updates the status of a booking (e.g., Active, CheckedIn, Completed, Cancelled).
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// PATCH /api/reception/bookings/1/status
    /// 
    /// {
    ///   "status": "Cancelled"
    /// }
    /// 
    /// Response example:
    /// 
    /// {
    ///   "bookingId": 1,
    ///   "status": "Cancelled"
    /// }
    /// </remarks>
    [HttpPatch("bookings/{id:int}/status")]
    public async Task<IActionResult> PatchBookingStatus(int id, [FromBody] PatchBookingStatusDto request, CancellationToken cancellationToken)
        => Ok(await commands.PatchBookingStatusAsync(id, request.Status, cancellationToken));

    /// <summary>
    /// Fully updates an existing booking.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// PUT /api/reception/bookings/1
    /// 
    /// {
    ///   "guestName": "John Doe",
    ///   "roomNumber": "101",
    ///   "status": "Active",
    ///   "checkInDate": "2026-06-01",
    ///   "checkOutDate": "2026-06-05"
    /// }
    /// 
    /// Response example:
    /// 
    /// {
    ///   "id": 1,
    ///   "guestName": "John Doe",
    ///   "roomNumber": "101",
    ///   "status": "Active",
    ///   "checkInDate": "2026-06-01",
    ///   "checkOutDate": "2026-06-05",
    ///   "total": 250.00
    /// }
    /// </remarks>
    [HttpPut("bookings/{id:int}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateBookingAsync(id, request, cancellationToken));

    /// <summary>
    /// Deletes a booking by its identifier.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// DELETE /api/reception/bookings/1
    /// 
    /// Response example:
    /// 
    /// Status: 204 No Content
    /// </remarks>
    [HttpDelete("bookings/{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteBookingAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Updates an existing guest's information.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// PUT /api/reception/guests/1
    /// 
    /// {
    ///   "fullName": "John Doe",
    ///   "email": "john@example.com"
    /// }
    /// 
    /// Response example:
    /// 
    /// {
    ///   "guestId": 1,
    ///   "fullName": "John Doe",
    ///   "email": "john@example.com"
    /// }
    /// </remarks>
    [HttpPut("guests/{id:int}")]
    public async Task<IActionResult> UpdateGuest(int id, [FromBody] UpdateGuestDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateGuestAsync(id, request, cancellationToken));

    /// <summary>
    /// Deletes a guest record by identifier.
    /// </summary>
    /// <remarks>
    /// Request example:
    /// 
    /// DELETE /api/reception/guests/1
    /// 
    /// Response example:
    /// 
    /// Status: 204 No Content
    /// </remarks>
    [HttpDelete("guests/{id:int}")]
    public async Task<IActionResult> DeleteGuest(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteGuestAsync(id, cancellationToken);
        return NoContent();
    }
}
