namespace HotelOS.RoomService.Models;

public enum RoomType
{
    Single,
    Double,
    Suite,
    Accessible
}

public sealed class Room
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType Type { get; set; } = RoomType.Single;
    public string Status { get; set; } = "Available";
    public decimal PricePerNight { get; set; }
    public int Floor { get; set; }
    public string Description { get; set; } = string.Empty;
    public int GuestCapacity { get; set; }
    public string MainImage { get; set; } = string.Empty;
    public string[] Images { get; set; } = Array.Empty<string>();
    public List<Amenity> Amenities { get; set; } = new();
}
