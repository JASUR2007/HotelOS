using Bogus;
using HotelOS.HousekeepingService.Data;
using HotelOS.HousekeepingService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.HousekeepingService.Seeders;

public static class DevelopmentSeeder
{
    private static readonly string[] StaffNames = [
        "Maria Lopez", "Sarah Kim", "James Wilson", "Ana Garcia", "Tom Brown",
        "Lisa Chen", "Mike Davis", "Eva Martinez", "John Taylor", "Nina Patel"
    ];

    private static readonly string[] Statuses = ["Queued", "In Progress", "Complete"];

    private static readonly string[] RoomStatusValues = ["Dirty", "Cleaning", "Clean", "Inspected"];

    public static async Task SeedAsync(HousekeepingDbContext context)
    {
        if (await context.CleaningTasks.CountAsync() > 10)
        {
            return;
        }

        var faker = new Faker();

        var tasks = new Faker<CleaningTask>()
            .RuleFor(item => item.RoomId, faker => faker.Random.Int(400, 999))
            .RuleFor(item => item.RoomNumber, (_, item) => item.RoomId.ToString())
            .RuleFor(item => item.Status, faker => faker.PickRandom(Statuses))
            .RuleFor(item => item.AssignedTo, faker => faker.PickRandom(StaffNames))
            .Generate(20);

        context.CleaningTasks.AddRange(tasks);
        await context.SaveChangesAsync();

        var roomStatuses = new Faker<RoomStatus>()
            .RuleFor(item => item.RoomId, faker => faker.Random.Int(400, 999))
            .RuleFor(item => item.RoomNumber, (_, item) => item.RoomId.ToString())
            .RuleFor(item => item.Status, faker => faker.PickRandom(RoomStatusValues))
            .Generate(15);

        context.RoomStatuses.AddRange(roomStatuses);
        await context.SaveChangesAsync();
    }
}
