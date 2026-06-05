using HotelOS.ReceptionService.Models;

namespace HotelOS.ReceptionService.Data.Factories;

public static class InvoiceFactory
{
    public static Invoice Create(int bookingId, decimal total) => new()
    {
        BookingId = bookingId,
        Total = total,
        Currency = "USD"
    };
}