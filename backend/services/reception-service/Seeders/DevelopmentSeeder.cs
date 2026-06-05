using Bogus;
using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Seeders;

public static class DevelopmentSeeder
{
    public static async Task SeedAsync(ReceptionDbContext context)
    {
        if (await context.Guests.CountAsync() > 10)
        {
            return;
        }

        var guestFaker = new Faker<Guest>()
            .RuleFor(item => item.FullName, faker => faker.Name.FullName())
            .RuleFor(item => item.Email, faker => faker.Internet.Email());

        var guests = guestFaker.Generate(25);
        context.Guests.AddRange(guests);
        await context.SaveChangesAsync();
    }
}