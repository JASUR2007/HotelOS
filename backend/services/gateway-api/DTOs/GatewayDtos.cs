namespace HotelOS.GatewayApi.DTOs;

/// <summary>A single metric entry for the dashboard summary grid.</summary>
/// <param name="Label">Display label for the metric (e.g. "Occupied Rooms").</param>
/// <param name="Value">Current value as a string (e.g. "28").</param>
/// <param name="Change">Change indicator text (e.g. "+5%", "-2").</param>
/// <param name="Tone">Trend tone: "up", "down", or "stable".</param>
/// <example>
/// {
///   "label": "Occupied Rooms",
///   "value": "28",
///   "change": "+5",
///   "tone": "up"
/// }
/// </example>
public sealed record DashboardSummaryDto(string Label, string Value, string Change, string Tone);