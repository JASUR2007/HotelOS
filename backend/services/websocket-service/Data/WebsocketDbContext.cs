using HotelOS.WebsocketService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelOS.WebsocketService.Data;

public sealed class WebsocketDbContext(DbContextOptions<WebsocketDbContext> options) : DbContext(options)
{
    public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("websocket");

        modelBuilder.Entity<NotificationRecord>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Type).HasMaxLength(50).IsRequired();
            entity.Property(item => item.Title).HasMaxLength(200).IsRequired();
            entity.Property(item => item.Message).HasMaxLength(500).IsRequired();
            entity.HasIndex(item => item.CreatedAt);
            entity.HasData(
                new NotificationRecord { Id = 1, Type = "reception", Title = "Guest checked in", Message = "Room assignment completed for Amelia Stone.", CreatedAt = new DateTimeOffset(2026, 5, 20, 9, 12, 0, TimeSpan.Zero) },
                new NotificationRecord { Id = 2, Type = "maintenance", Title = "Maintenance escalated", Message = "Room 302 moved to critical priority.", CreatedAt = new DateTimeOffset(2026, 5, 20, 9, 5, 0, TimeSpan.Zero) }
            );
        });
    }
}