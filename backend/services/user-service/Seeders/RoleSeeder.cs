using HotelOS.UserService.Data;
using HotelOS.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.UserService.Seeders;

public static class RoleSeeder
{
    private static readonly Dictionary<string, string[]> RolePermissions = new()
    {
        ["SuperAdmin"] = ["create_booking", "update_booking", "delete_booking", "manage_rooms", "manage_users", "manage_roles", "manage_permissions", "create_orders", "update_orders", "resolve_maintenance", "assign_maintenance", "view_dashboard", "view_settings", "view_maintenances", "view_housekeeping", "manage_payments", "view_reports", "view_audit_logs", "view_event_logs", "process_refunds"],
        ["Admin"] = ["create_booking", "update_booking", "manage_rooms", "manage_users", "view_dashboard", "view_settings", "view_maintenances", "view_housekeeping", "view_reports", "view_audit_logs"],
        ["Receptionist"] = ["create_booking", "update_booking", "view_dashboard", "view_maintenances"],
        ["Housekeeper"] = ["view_dashboard", "view_housekeeping"],
        ["Technician"] = ["view_dashboard", "view_maintenances"],
        ["KitchenStaff"] = ["view_dashboard"],
        ["Accountant"] = ["view_dashboard", "manage_payments", "view_reports", "process_refunds"],
        ["Guest"] = []
    };

    private static readonly string[] RoleNames = [..RolePermissions.Keys];

    public static async Task SeedAsync(UserDbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var roleName in RoleNames)
        {
            var role = await context.Roles.FirstOrDefaultAsync(item => item.Name == roleName, cancellationToken);
            if (role is null)
            {
                role = new AppRole { Name = roleName };
                context.Roles.Add(role);
                await context.SaveChangesAsync(cancellationToken);
            }

            var desiredPermNames = RolePermissions[roleName];
            var desiredPermIds = new HashSet<int>();
            foreach (var permName in desiredPermNames)
            {
                var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == permName, cancellationToken);
                if (permission is not null)
                    desiredPermIds.Add(permission.Id);
            }

            var existingBridges = await context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .ToListAsync(cancellationToken);

            var toRemove = existingBridges.Where(rp => !desiredPermIds.Contains(rp.PermissionId)).ToList();
            if (toRemove.Count > 0)
            {
                context.RolePermissions.RemoveRange(toRemove);
            }

            var existingPermIds = existingBridges.Select(rp => rp.PermissionId).ToHashSet();
            foreach (var permId in desiredPermIds)
            {
                if (!existingPermIds.Contains(permId))
                {
                    context.RolePermissions.Add(new RolePermissionBridge { RoleId = role.Id, PermissionId = permId });
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}