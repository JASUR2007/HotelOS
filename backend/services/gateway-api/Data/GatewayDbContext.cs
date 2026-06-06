using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotelOS.GatewayApi.Models;

namespace HotelOS.GatewayApi.Data;

public sealed class GatewayDbContext(DbContextOptions<GatewayDbContext> options) : DbContext(options)
{
    public DbSet<GatewayAuditLog> AuditLogs => Set<GatewayAuditLog>();
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<ApplicationRole> Roles => Set<ApplicationRole>();
    public DbSet<ApplicationPermission> Permissions => Set<ApplicationPermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("audit");

        modelBuilder.Entity<GatewayAuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.UserId).HasMaxLength(50);
            entity.Property(item => item.UserName).HasMaxLength(200);
            entity.Property(item => item.Action).HasMaxLength(500).IsRequired();
            entity.Property(item => item.EntityType).HasMaxLength(100);
            entity.Property(item => item.EntityId).HasMaxLength(50);
            entity.Property(item => item.IpAddress).HasMaxLength(50);
            entity.Property(item => item.ServiceName).HasMaxLength(100);
            entity.Property(item => item.CreatedAt).IsRequired();
            entity.HasIndex(item => item.CreatedAt);
            entity.HasIndex(item => item.EntityType);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("users", "users");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Email).HasMaxLength(200).IsRequired();
            entity.Property(item => item.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(item => item.PasswordHash).HasMaxLength(200).IsRequired();
            entity.HasIndex(item => item.Email).IsUnique();
            entity.HasData(
                new ApplicationUser { Id = 1, Email = "admin@hotelos.local", DisplayName = "Super Admin", PasswordHash = "admin123", IsActive = true },
                new ApplicationUser { Id = 2, Email = "reception@hotelos.local", DisplayName = "Reception Desk", PasswordHash = "reception123", IsActive = true },
                new ApplicationUser { Id = 3, Email = "housekeeping@hotelos.local", DisplayName = "Housekeeping Lead", PasswordHash = "house123", IsActive = true }
            );
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("roles", "users");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(item => item.Name).IsUnique();
            entity.HasData(
                new ApplicationRole { Id = 1, Name = "SuperAdmin" },
                new ApplicationRole { Id = 2, Name = "Admin" },
                new ApplicationRole { Id = 3, Name = "Receptionist" },
                new ApplicationRole { Id = 4, Name = "Housekeeper" },
                new ApplicationRole { Id = 5, Name = "Technician" },
                new ApplicationRole { Id = 6, Name = "KitchenStaff" },
                new ApplicationRole { Id = 7, Name = "Guest" }
            );
        });

        modelBuilder.Entity<ApplicationPermission>(entity =>
        {
            entity.ToTable("permissions", "users");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(100).IsRequired();
            entity.Property(item => item.Description).HasMaxLength(250).IsRequired();
            entity.HasIndex(item => item.Name).IsUnique();
            entity.HasData(
                new ApplicationPermission { Id = 1, Name = "create_booking", Description = "Create new booking records" },
                new ApplicationPermission { Id = 2, Name = "update_booking", Description = "Update booking details" },
                new ApplicationPermission { Id = 3, Name = "delete_booking", Description = "Delete bookings" },
                new ApplicationPermission { Id = 4, Name = "manage_rooms", Description = "Manage room inventory and status" },
                new ApplicationPermission { Id = 5, Name = "manage_users", Description = "Manage system users" },
                new ApplicationPermission { Id = 6, Name = "manage_roles", Description = "Manage role assignments" },
                new ApplicationPermission { Id = 7, Name = "manage_permissions", Description = "Manage permission catalog" },
                new ApplicationPermission { Id = 8, Name = "create_orders", Description = "Create room service orders" },
                new ApplicationPermission { Id = 9, Name = "update_orders", Description = "Update room service orders" },
                new ApplicationPermission { Id = 10, Name = "resolve_maintenance", Description = "Resolve maintenance issues" },
                new ApplicationPermission { Id = 11, Name = "assign_maintenance", Description = "Assign maintenance technicians" },
                new ApplicationPermission { Id = 12, Name = "view_dashboard", Description = "View admin dashboard" },
                new ApplicationPermission { Id = 13, Name = "manage_payments", Description = "Manage payments and billing" },
                new ApplicationPermission { Id = 14, Name = "view_reports", Description = "View and export reports" },
                new ApplicationPermission { Id = 15, Name = "manage_workers", Description = "Monitor workforce status" },
                new ApplicationPermission { Id = 16, Name = "view_maintenances", Description = "View maintenance requests" },
                new ApplicationPermission { Id = 17, Name = "view_housekeeping", Description = "View housekeeping status" },
                new ApplicationPermission { Id = 18, Name = "view_audit_logs", Description = "View audit logs" },
                new ApplicationPermission { Id = 19, Name = "view_event_logs", Description = "View event logs" },
                new ApplicationPermission { Id = 20, Name = "process_refunds", Description = "Process refunds" }
            );
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("users_bridge", "users");
            entity.HasKey(item => new { item.UserId, item.RoleId });
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(item => item.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ApplicationRole>().WithMany().HasForeignKey(item => item.RoleId).OnDelete(DeleteBehavior.Cascade);
            entity.HasData(
                new UserRole { UserId = 1, RoleId = 1 },
                new UserRole { UserId = 2, RoleId = 3 },
                new UserRole { UserId = 3, RoleId = 4 }
            );
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions", "users");
            entity.HasKey(item => new { item.RoleId, item.PermissionId });
            entity.HasOne<ApplicationRole>().WithMany().HasForeignKey(item => item.RoleId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ApplicationPermission>().WithMany().HasForeignKey(item => item.PermissionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasData(
                new RolePermission { RoleId = 1, PermissionId = 1 }, new RolePermission { RoleId = 1, PermissionId = 2 }, new RolePermission { RoleId = 1, PermissionId = 3 },
                new RolePermission { RoleId = 1, PermissionId = 4 }, new RolePermission { RoleId = 1, PermissionId = 5 }, new RolePermission { RoleId = 1, PermissionId = 6 },
                new RolePermission { RoleId = 1, PermissionId = 7 }, new RolePermission { RoleId = 1, PermissionId = 8 }, new RolePermission { RoleId = 1, PermissionId = 9 },
                new RolePermission { RoleId = 1, PermissionId = 10 }, new RolePermission { RoleId = 1, PermissionId = 11 }, new RolePermission { RoleId = 1, PermissionId = 12 },
                new RolePermission { RoleId = 1, PermissionId = 13 }, new RolePermission { RoleId = 1, PermissionId = 14 },
                new RolePermission { RoleId = 3, PermissionId = 1 }, new RolePermission { RoleId = 3, PermissionId = 2 }, new RolePermission { RoleId = 3, PermissionId = 12 }, new RolePermission { RoleId = 3, PermissionId = 13 },
                new RolePermission { RoleId = 4, PermissionId = 12 }
            );
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens", "users");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Token).HasMaxLength(250).IsRequired();
            entity.HasIndex(item => item.Token).IsUnique();
        });
    }
}