using HotelOS.WebsocketService.DTOs;
using HotelOS.WebsocketService.Repositories;
using HotelOS.WebsocketService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.WebsocketService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotificationsController(
    INotificationRepository repository,
    INotificationBroadcastService broadcaster) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        return Ok(await repository.GetAllAsync(role, cancellationToken));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RealtimeNotificationDto notification, CancellationToken cancellationToken)
    {
        await broadcaster.BroadcastAsync(notification, cancellationToken);
        return Accepted(notification);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        await repository.MarkAsReadAsync(id, cancellationToken);
        return Ok();
    }
}