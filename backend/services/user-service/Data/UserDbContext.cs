using HotelOS.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.UserService.Data;

public sealed class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<AppPermission> Permissions => Set<AppPermission>();
    public DbSet<UserRoleBridge> UserRoles => Set<UserRoleBridge>();
    public DbSet<RolePermissionBridge> RolePermissions => Set<RolePermissionBridge>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginHistory> LoginHistory => Set<LoginHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Email).HasMaxLength(200).IsRequired();
            entity.Property(item => item.PasswordHash).HasMaxLength(250).IsRequired();
            entity.Property(item => item.DisplayName).HasMaxLength(200).IsRequired();
            entity.HasIndex(item => item.Email).IsUnique();
        });

        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<AppPermission>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(100).IsRequired();
            entity.Property(item => item.Description).HasMaxLength(250).IsRequired();
            entity.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<UserRoleBridge>(entity =>
        {
            entity.ToTable("users_bridge");
            entity.HasKey(item => new { item.UserId, item.RoleId });
        });

        modelBuilder.Entity<RolePermissionBridge>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(item => new { item.RoleId, item.PermissionId });
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.Token).IsUnique();
        });

        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.ToTable("login_history");
            entity.HasKey(item => item.Id);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(item => item.Id);
        });
    }
}