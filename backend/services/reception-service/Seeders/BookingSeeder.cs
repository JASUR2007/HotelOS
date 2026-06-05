using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Seeders;

public static class BookingSeeder
{
    public static async Task SeedAsync(ReceptionDbContext context)
    {
        if (await context.Bookings.AnyAsync())
        {
            return;
        }

        context.Bookings.AddRange(
            new Booking { Id = 1, GuestId = 1, RoomId = 101, CheckInDate = new DateOnly(2026, 5, 20), CheckOutDate = new DateOnly(2026, 5, 23), Status = "CheckedIn" },
            new Booking { Id = 2, GuestId = 2, RoomId = 207, CheckInDate = new DateOnly(2026, 5, 19), CheckOutDate = new DateOnly(2026, 5, 24), Status = "Booked" }
        );

        await context.SaveChangesAsync();
    }
}