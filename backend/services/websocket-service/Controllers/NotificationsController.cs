using HotelOS.WebsocketService.DTOs;
using HotelOS.WebsocketService.Repositories;
using HotelOS.WebsocketService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.WebsocketService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotificationsController(
    INotificationRepository repository,
    INotificationBroadcastService broadcaster) : ControllerBase
{
    /// <summary>Retrieves all notifications.</summary>
    /// <remarks>
    /// Example request: GET /api/notifications
    ///
    /// Example response:
    /// <code>
    /// [
    ///   {
    ///     "id": "1",
    ///     "title": "New Booking",
    ///     "message": "Room 101 has been booked by John Doe",
    ///     "createdAt": "2026-06-01T10:00:00Z",
    ///     "type": "Booking"
    ///   }
    /// ]
    /// </code>
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await repository.GetAllAsync(cancellationToken));

    /// <summary>Broadcasts a notification to all connected WebSocket clients.</summary>
    /// <remarks>
    /// Example request: POST /api/notifications
    /// <code>
    /// {
    ///   "id": "2",
    ///   "title": "Maintenance Update",
    ///   "message": "Room 204 requires maintenance",
    ///   "createdAt": "2026-06-01T11:00:00Z",
    ///   "type": "Maintenance"
    /// }
    /// </code>
    /// Example response: 202 Accepted
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RealtimeNotificationDto notification, CancellationToken cancellationToken)
    {
        await broadcaster.BroadcastAsync(notification, cancellationToken);
        return Accepted(notification);
    }
}