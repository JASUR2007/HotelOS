using HotelOS.MaintenanceService.Models;

namespace HotelOS.MaintenanceService.Data.Factories;

public static class TechnicianFactory
{
    public static TechnicianAssignment Create(int issueId, string technicianName) => new()
    {
        IssueId = issueId,
        TechnicianName = technicianName
    };
}