namespace HotelOS.RoomService.Models;

public sealed class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public sealed class FoodOrder
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string Status { get; set; } = "Preparing";
    public decimal Total { get; set; }
}