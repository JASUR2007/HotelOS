using HotelOS.GatewayApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/admin/audit-logs")]
public sealed class AuditLogsController(GatewayDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(CancellationToken cancellationToken)
    {
        try
        {
            var logs = await context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(l => l.CreatedAt)
                .Take(200)
                .Select(l => new
                {
                    l.Id,
                    l.UserName,
                    l.Action,
                    l.EntityType,
                    l.EntityId,
                    l.IpAddress,
                    l.ServiceName,
                    l.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return Ok(logs);
        }
        catch
        {
            return Ok(Array.Empty<object>());
        }
    }
}
