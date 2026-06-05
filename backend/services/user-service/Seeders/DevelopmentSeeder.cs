using HotelOS.UserService.Data;
using HotelOS.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.UserService.Seeders;

public static class DevelopmentSeeder
{
    public static async Task SeedAsync(UserDbContext context)
    {
        if (await context.LoginHistory.CountAsync() > 10)
        {
            return;
        }

        context.LoginHistory.AddRange(
            new LoginHistory { UserId = 1, IpAddress = "127.0.0.1", LoggedInAt = DateTimeOffset.UtcNow.AddMinutes(-25) },
            new LoginHistory { UserId = 2, IpAddress = "127.0.0.1", LoggedInAt = DateTimeOffset.UtcNow.AddMinutes(-15) },
            new LoginHistory { UserId = 3, IpAddress = "127.0.0.1", LoggedInAt = DateTimeOffset.UtcNow.AddMinutes(-5) }
        );

        await context.SaveChangesAsync();
    }
}