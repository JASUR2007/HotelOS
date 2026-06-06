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
    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings(CancellationToken cancellationToken)
        => Ok(await queries.GetBookingsAsync(cancellationToken));

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request, CancellationToken cancellationToken)
        => Ok(await commands.CheckInAsync(request, cancellationToken));

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequestDto request, CancellationToken cancellationToken)
        => Ok(await commands.CheckOutAsync(request, cancellationToken));

    [HttpPost("hold")]
    public async Task<IActionResult> HoldReservation([FromBody] HoldReservationDto request, CancellationToken cancellationToken)
        => Ok(await commands.HoldReservationAsync(request, cancellationToken));

    [HttpGet("guests")]
    public async Task<IActionResult> GetGuests(CancellationToken cancellationToken)
        => Ok(await queries.GetGuestsAsync(cancellationToken));

    [HttpPost("guests")]
    public async Task<IActionResult> CreateGuest([FromBody] CreateGuestDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateGuestAsync(request, cancellationToken));

    [HttpGet("my-reservations")]
    public async Task<IActionResult> GetMyReservations([FromQuery] string? email, CancellationToken cancellationToken)
        => Ok(await queries.GetReservationsByEmailAsync(email, cancellationToken));

    [HttpGet("bookings/expired")]
    public async Task<IActionResult> GetExpiredBookings([FromQuery] string status, [FromQuery] int minutes, CancellationToken cancellationToken)
        => Ok(await queries.GetExpiredBookingsAsync(status, minutes, cancellationToken));

    [HttpPatch("bookings/{id:int}/status")]
    public async Task<IActionResult> PatchBookingStatus(int id, [FromBody] PatchBookingStatusDto request, CancellationToken cancellationToken)
        => Ok(await commands.PatchBookingStatusAsync(id, request.Status, cancellationToken));

    [HttpPost("bookings/{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id, CancellationToken cancellationToken)
        => Ok(await commands.CancelBookingAsync(id, cancellationToken));

    [HttpPut("bookings/{id:int}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateBookingAsync(id, request, cancellationToken));

    [HttpDelete("bookings/{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteBookingAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("guests/{id:int}")]
    public async Task<IActionResult> UpdateGuest(int id, [FromBody] UpdateGuestDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateGuestAsync(id, request, cancellationToken));

    [HttpDelete("guests/{id:int}")]
    public async Task<IActionResult> DeleteGuest(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteGuestAsync(id, cancellationToken);
        return NoContent();
    }
}