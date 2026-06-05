using HotelOS.UserService.Data;
using HotelOS.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.UserService.Seeders;

public static class PermissionSeeder
{
    private static readonly (string Name, string Description)[] Permissions =
    [
        ("create_booking", "Create bookings"),
        ("update_booking", "Update bookings"),
        ("delete_booking", "Delete bookings"),
        ("manage_rooms", "Manage rooms"),
        ("manage_users", "Manage users"),
        ("manage_roles", "Manage roles"),
        ("manage_permissions", "Manage permissions"),
        ("view_dashboard", "View dashboard"),
        ("view_settings", "View settings"),
        ("view_maintenances", "View maintenances"),
        ("manage_payments", "Manage payments"),
        ("view_reports", "View reports"),
        ("view_audit_logs", "View audit logs"),
        ("view_event_logs", "View event logs"),
        ("process_refunds", "Process refunds")
    ];

    public static async Task SeedAsync(UserDbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var (name, description) in Permissions)
        {
            if (await context.Permissions.AnyAsync(item => item.Name == name, cancellationToken))
            {
                continue;
            }

            context.Permissions.Add(new AppPermission { Name = name, Description = description });
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}