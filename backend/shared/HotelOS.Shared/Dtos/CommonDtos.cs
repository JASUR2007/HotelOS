namespace HotelOS.Shared.Dtos;

public sealed record ApiResultDto(bool Success, string Message);

public sealed record DashboardMetricDto(string Label, string Value, string Change, string Tone);

public sealed record RealtimeNotificationDto(string Id, string Title, string Message, string CreatedAt, string Type);