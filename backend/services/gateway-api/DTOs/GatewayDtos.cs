namespace HotelOS.GatewayApi.DTOs;

/// <param name="Label">Metric display label (e.g. "Occupied Rooms").</param>
/// <param name="Value">Current value as string.</param>
/// <param name="Change">Change indicator (e.g. "+5%").</param>
/// <param name="Tone">Trend: "up", "down", or "stable".</param>
public sealed record DashboardSummaryDto(string Label, string Value, string Change, string Tone);