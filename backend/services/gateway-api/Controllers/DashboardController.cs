using HotelOS.GatewayApi.Services;
using HotelOS.GatewayApi.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequiresPermission("view_dashboard")]
public sealed class DashboardController(IGatewayService service) : ControllerBase
{
    /// <summary>Returns the main hotel dashboard summary with key operational metrics.</summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/dashboard/summary
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "totalBookings": 42,
    ///       "occupancyRate": 78.5,
    ///       "activeMaintenance": 3,
    ///       "pendingHousekeeping": 5,
    ///       "revenueToday": 1250.00,
    ///       "revenueThisMonth": 45230.00,
    ///       "availableRooms": 12,
    ///       "occupiedRooms": 28,
    ///       "lastUpdated": "2026-06-01T10:00:00Z"
    ///     }
    /// </remarks>
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
        => Ok(await service.GetDashboardSummaryAsync(cancellationToken));
}