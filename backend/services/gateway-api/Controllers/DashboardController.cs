using HotelOS.GatewayApi.Services;
using HotelOS.GatewayApi.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequiresPermission("view_dashboard")]
public sealed class DashboardController(IGatewayService service) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
        => Ok(await service.GetDashboardSummaryAsync(cancellationToken));
}