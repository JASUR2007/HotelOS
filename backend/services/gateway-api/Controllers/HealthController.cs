using HotelOS.GatewayApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController(IGatewayRepository repository) : ControllerBase
{
    /// <summary>Checks the health status of the gateway and downstream services.</summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/health
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "status": "Healthy",
    ///       "timestamp": "2026-06-01T10:00:00Z",
    ///       "services": [
    ///         { "name": "room-service", "status": "Healthy", "responseTimeMs": 45 },
    ///         { "name": "booking-service", "status": "Healthy", "responseTimeMs": 62 }
    ///       ]
    ///     }
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await repository.GetServiceHealthAsync(cancellationToken);
        return Ok(result);
    }
}