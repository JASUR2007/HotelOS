namespace HotelOS.RoomService.Models;

public sealed class RoomKey
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string KeyType { get; set; } = "Room"; // Room, Master
    public string Status { get; set; } = "Available"; // Available, Issued, Returned, Lost
    public string? IssuedTo { get; set; }
    public string? IssuedBy { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
    public DateTimeOffset? ReturnedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class MasterKey
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AccessScope { get; set; } = string.Empty; // e.g. "1-3", "All", "Suite"
    public string Status { get; set; } = "Available";
    public string? IssuedTo { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
    public DateTimeOffset? ReturnedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
