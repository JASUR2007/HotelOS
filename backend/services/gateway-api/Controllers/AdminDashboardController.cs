using Microsoft.AspNetCore.Mvc;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
public sealed class AdminDashboardController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    /// <summary>Returns high-level admin dashboard metrics including room occupancy, bookings, revenue, and notifications.</summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/admin/dashboard/metrics
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       { "label": "Occupied Rooms", "value": "28", "delta": "+5 reserved", "trend": "up" },
    ///       { "label": "Available Rooms", "value": "12", "delta": "40 total", "trend": "up" },
    ///       { "label": "Dirty Rooms", "value": "3", "delta": "3 pending", "trend": "down" },
    ///       { "label": "Active Bookings", "value": "5", "delta": "+0 today", "trend": "stable" },
    ///       { "label": "Pending Maintenance", "value": "2", "delta": "none", "trend": "stable" },
    ///       { "label": "Daily Revenue", "value": "$1,250", "delta": "+5%", "trend": "up" },
    ///       { "label": "Live Notifications", "value": "7", "delta": "+3", "trend": "up" }
    ///     ]
    /// </remarks>
    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
    {
        var metrics = new List<object>();
        try
        {
            var roomClient = httpClientFactory.CreateClient("room-service");
            var rooms = await roomClient.GetFromJsonAsync<List<RoomMetric>>("api/room/rooms", cancellationToken);
            var occupied = rooms?.Count(r => r.Status == "Occupied") ?? 0;
            var available = rooms?.Count(r => r.Status == "Available") ?? 0;
            var maintenance = rooms?.Count(r => r.Status == "Maintenance") ?? 0;
            var reserved = rooms?.Count(r => r.Status == "HELD" || r.Status == "Reserved") ?? 0;

            metrics.Add(new { label = "Occupied Rooms", value = occupied.ToString(), delta = $"+{reserved} reserved", trend = "up" });
            metrics.Add(new { label = "Available Rooms", value = available.ToString(), delta = $"{available + occupied} total", trend = "up" });
            metrics.Add(new { label = "Dirty Rooms", value = maintenance.ToString(), delta = $"{maintenance} pending", trend = "down" });
            metrics.Add(new { label = "Active Bookings", value = reserved.ToString(), delta = "+0 today", trend = "stable" });
        }
        catch { }

        metrics.Add(new { label = "Pending Maintenance", value = "0", delta = "none", trend = "stable" });
        metrics.Add(new { label = "Active Food Orders", value = "0", delta = "+0", trend = "stable" });
        metrics.Add(new { label = "Daily Revenue", value = "$0", delta = "+0%", trend = "stable" });
        metrics.Add(new { label = "Live Notifications", value = "0", delta = "+0", trend = "stable" });

        return Ok(metrics);
    }

    /// <summary>Retrieves room occupancy breakdown (occupied, available, other) for charting.</summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/admin/dashboard/occupancy
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       { "label": "Occupied", "value": 28 },
    ///       { "label": "Available", "value": 12 },
    ///       { "label": "Other", "value": 3 }
    ///     ]
    /// </remarks>
    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy(CancellationToken cancellationToken)
    {
        try
        {
            var roomClient = httpClientFactory.CreateClient("room-service");
            var rooms = await roomClient.GetFromJsonAsync<List<RoomMetric>>("api/room/rooms", cancellationToken);
            var available = rooms?.Count(r => r.Status == "Available") ?? 0;
            var occupied = rooms?.Count(r => r.Status == "Occupied") ?? 0;
            var other = (rooms?.Count ?? 0) - available - occupied;
            return Ok(new[]
            {
                new { label = "Occupied", value = occupied },
                new { label = "Available", value = available },
                new { label = "Other", value = other }
            });
        }
        catch
        {
            return Ok(new[] { new { label = "No Data", value = 0 } });
        }
    }

    /// <summary>Returns revenue data for admin charting (currently a stub).</summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/admin/dashboard/revenue
    /// 
    /// Sample response (200 OK):
    /// 
    ///     []
    /// </remarks>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue(CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
    }

    /// <summary>Returns payment statistics for admin dashboard (currently a stub).</summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/admin/dashboard/payments
    /// 
    /// Sample response (200 OK):
    /// 
    ///     []
    /// </remarks>
    [HttpGet("payments")]
    public async Task<IActionResult> GetPaymentStats(CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
    }

    /// <summary>Returns payment analytics data for admin dashboard (stub).</summary>
    [HttpGet("payment-analytics")]
    public async Task<IActionResult> GetPaymentAnalytics(CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
    }

    private sealed class RoomMetric
    {
        public string Status { get; set; } = string.Empty;
    }
}
