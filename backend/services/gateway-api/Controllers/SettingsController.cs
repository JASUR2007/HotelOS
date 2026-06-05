using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/admin/settings")]
public sealed class SettingsController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, string> Settings = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Notifications"] = "Enabled",
        ["Payments"] = "Live",
        ["RBAC"] = "Strict",
        ["RabbitMQ"] = "Healthy",
        ["Database"] = "Healthy",
        ["WebSockets"] = "Healthy",
        ["Event Logs"] = "Healthy"
    };

    private static readonly Dictionary<string, string> Descriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Notifications"] = "Realtime alerts via SignalR",
        ["Payments"] = "Payment provider and reconciliation state",
        ["RBAC"] = "Role and permission enforcement enabled",
        ["RabbitMQ"] = "Queues, consumers, messages",
        ["Database"] = "Postgres status, connections",
        ["WebSockets"] = "SignalR connections",
        ["Event Logs"] = "Routing and delivery"
    };

    [HttpGet]
    public IActionResult Get()
        => Ok(Settings.Select(setting => new
        {
            key = setting.Key,
            value = setting.Value,
            description = Descriptions.GetValueOrDefault(setting.Key, "")
        }));

    [HttpPut("{key}")]
    public IActionResult Update(string key, [FromBody] UpdateSettingRequest request)
    {
        if (!Descriptions.ContainsKey(key))
            return NotFound(new { message = "Setting not found" });

        if (!IsAllowedValue(key, request.Value))
            return BadRequest(new { message = "Invalid setting value" });

        Settings[key] = request.Value;
        return Ok(new { key, value = request.Value, description = Descriptions[key] });
    }

    private static bool IsAllowedValue(string key, string value) => key.ToLowerInvariant() switch
    {
        "notifications" => value is "Enabled" or "Disabled",
        "payments" => value is "Live" or "Test" or "Disabled",
        "rbac" => value is "Strict" or "Standard" or "Disabled",
        "rabbitmq" or "database" or "websockets" or "event logs" => value is "Healthy" or "Degraded" or "Maintenance" or "Offline",
        _ => false
    };
}

public sealed record UpdateSettingRequest(string Value);
