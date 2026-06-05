namespace HotelOS.MaintenanceService.DTOs;

public sealed record MaintenanceIssueDto
{
    public MaintenanceIssueDto(int id, string roomNumber, string title, string priority, string status, string? technicianName)
    {
        Id = id;
        RoomNumber = roomNumber;
        Title = title;
        Priority = priority;
        Status = status;
        TechnicianName = technicianName;
    }

    public int Id { get; init; }
    public string RoomNumber { get; init; }
    public string Title { get; init; }
    public string Priority { get; init; }
    public string Status { get; init; }
    public string? TechnicianName { get; init; }
}

public sealed record CreateIssueDto
{
    public string RoomNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
}

public sealed record AssignTechnicianDto
{
    public int IssueId { get; init; }
    public string TechnicianName { get; init; } = string.Empty;
}

public sealed record UpdateIssueDto
{
    public string? Status { get; init; }
    public string? TechnicianName { get; init; }
    public string? Priority { get; init; }
}