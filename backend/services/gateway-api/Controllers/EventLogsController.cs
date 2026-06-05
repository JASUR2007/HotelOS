using HotelOS.GatewayApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/admin/event-logs")]
public sealed class EventLogsController(IEventLogRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEventLogs(CancellationToken cancellationToken)
    {
        try
        {
            var logs = await repo.GetAllAsync(cancellationToken);
            return Ok(logs.Select(l => new
            {
                l.Id,
                eventName = l.Action,
                routingKey = l.EntityType,
                status = "processed",
                createdAt = l.CreatedAt
            }));
        }
        catch
        {
            return Ok(Array.Empty<object>());
        }
    }
}
