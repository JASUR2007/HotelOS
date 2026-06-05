using HotelOS.UserService.Data;
using HotelOS.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.UserService.Seeders;

public static class AdminSeeder
{
    public static async Task SeedAsync(UserDbContext context, CancellationToken cancellationToken = default)
    {
        await SeedUserAsync(context, "admin@hotelos.local", "Super Admin", "admin123", ["SuperAdmin"], cancellationToken);
        await SeedUserAsync(context, "reception@hotelos.local", "Reception Desk", "reception123", ["Receptionist"], cancellationToken);
        await SeedUserAsync(context, "accountant@hotelos.local", "Accountant", "account123", ["Accountant"], cancellationToken);
    }

    private static async Task SeedUserAsync(
        UserDbContext context,
        string email,
        string displayName,
        string password,
        IReadOnlyCollection<string> roleNames,
        CancellationToken cancellationToken)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);
        if (existingUser is null)
        {
            existingUser = new AppUser
            {
                Email = email,
                DisplayName = displayName,
                PasswordHash = password,
                Status = "Active"
            };

            context.Users.Add(existingUser);
            await context.SaveChangesAsync(cancellationToken);
        }

        foreach (var roleName in roleNames)
        {
            var role = await context.Roles.FirstAsync(item => item.Name == roleName, cancellationToken);
            var alreadyAssigned = await context.UserRoles.AnyAsync(item => item.UserId == existingUser.Id && item.RoleId == role.Id, cancellationToken);
            if (!alreadyAssigned)
            {
                context.UserRoles.Add(new UserRoleBridge { UserId = existingUser.Id, RoleId = role.Id });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}