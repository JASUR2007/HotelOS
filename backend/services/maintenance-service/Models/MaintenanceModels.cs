namespace HotelOS.MaintenanceService.Models;

public sealed class MaintenanceIssue
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string Priority { get; set; } = "Normal";
    public string Status { get; set; } = "Queued";
    public string TechnicianName { get; set; } = "Unassigned";
}

public sealed class TechnicianAssignment
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
}