namespace HotelOS.HousekeepingService.Models;

public sealed class CleaningTask
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Queued";
    public string AssignedTo { get; set; } = "Unassigned";
    public string Priority { get; set; } = "Normal";
}

public sealed class RoomStatus
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Dirty";
}