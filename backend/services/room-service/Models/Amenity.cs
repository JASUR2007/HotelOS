using System.Text.Json.Serialization;

namespace HotelOS.RoomService.Models;

public sealed class Amenity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [JsonIgnore]
    public List<Room> Rooms { get; set; } = new();
}
