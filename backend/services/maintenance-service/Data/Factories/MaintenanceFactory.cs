using HotelOS.MaintenanceService.Models;

namespace HotelOS.MaintenanceService.Data.Factories;

public static class MaintenanceFactory
{
    public static MaintenanceIssue Create(string roomNumber, string title, string priority, string status) => new()
    {
        RoomNumber = roomNumber,
        Title = title,
        Priority = priority,
        Status = status
    };
}