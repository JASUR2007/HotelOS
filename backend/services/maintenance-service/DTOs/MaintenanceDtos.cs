namespace HotelOS.MaintenanceService.DTOs;

/// <summary>Represents a maintenance issue returned from the API.</summary>
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

    /// <example>1</example>
    public int Id { get; init; }
    /// <example>101</example>
    public string RoomNumber { get; init; }
    /// <example>Air conditioning not working</example>
    public string Title { get; init; }
    /// <example>High</example>
    public string Priority { get; init; }
    /// <example>InProgress</example>
    public string Status { get; init; }
    /// <example>Bob Johnson</example>
    public string? TechnicianName { get; init; }
}

/// <summary>Data required to create a new maintenance issue.</summary>
public sealed record CreateIssueDto
{
    /// <example>101</example>
    public string RoomNumber { get; init; } = string.Empty;

    /// <example>Air conditioning not working</example>
    public string Title { get; init; } = string.Empty;

    /// <example>Medium</example>
    public string Priority { get; init; } = string.Empty;
}

/// <summary>Data required to assign a technician to an issue.</summary>
public sealed record AssignTechnicianDto
{
    /// <example>1</example>
    public int IssueId { get; init; }

    /// <example>Bob Johnson</example>
    public string TechnicianName { get; init; } = string.Empty;
}

/// <summary>Data for updating an existing maintenance issue. All fields are optional.</summary>
public sealed record UpdateIssueDto
{
    /// <example>Completed</example>
    public string? Status { get; init; }

    /// <example>Bob Johnson</example>
    public string? TechnicianName { get; init; }

    /// <example>Low</example>
    public string? Priority { get; init; }
}