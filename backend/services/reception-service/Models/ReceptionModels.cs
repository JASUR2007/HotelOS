using System.ComponentModel.DataAnnotations;

namespace HotelOS.ReceptionService.Models;

public sealed class Guest
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public sealed class Booking
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int GuestId { get; set; }
    public int RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string Status { get; set; } = "Booked";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public byte[]? RowVersion { get; set; }
}

public sealed class Invoice
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = "USD";
}