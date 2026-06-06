using HotelOS.GatewayApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController(IGatewayRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await repository.GetServiceHealthAsync(cancellationToken);
        return Ok(result);
    }
}