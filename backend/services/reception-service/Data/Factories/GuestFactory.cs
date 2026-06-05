using Bogus;
using HotelOS.ReceptionService.Models;

namespace HotelOS.ReceptionService.Data.Factories;

public static class GuestFactory
{
    public static Faker<Guest> Create() => new Faker<Guest>()
        .RuleFor(item => item.FullName, faker => faker.Name.FullName())
        .RuleFor(item => item.Email, faker => faker.Internet.Email());
}