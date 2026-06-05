namespace HotelOS.MaintenanceService.Events;

public sealed record MaintenanceIssueRaisedEvent(int IssueId, string RoomNumber, DateTimeOffset OccurredAt);
public sealed record MaintenanceIssueAssignedEvent(int IssueId, string TechnicianName, DateTimeOffset OccurredAt);