using HotelOS.MaintenanceService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelOS.MaintenanceService.Data;

public sealed class MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : DbContext(options)
{
    public DbSet<MaintenanceIssue> MaintenanceIssues => Set<MaintenanceIssue>();
    public DbSet<TechnicianAssignment> TechnicianAssignments => Set<TechnicianAssignment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("maintenance");

        modelBuilder.Entity<MaintenanceIssue>(entity =>
        {
            entity.ToTable("issues");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.RoomNumber).HasMaxLength(20).IsRequired();
            entity.Property(item => item.Title).HasMaxLength(200).IsRequired();
            entity.Property(item => item.Priority).HasMaxLength(20).IsRequired();
            entity.Property(item => item.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(item => new { item.Priority, item.Status });
            entity.HasData(
                new MaintenanceIssue { Id = 1, RoomNumber = "302", Title = "AC not cooling", Priority = "Critical", Status = "Assigned" },
                new MaintenanceIssue { Id = 2, RoomNumber = "118", Title = "Bathroom light flickering", Priority = "Medium", Status = "Queued" }
            );
        });

        modelBuilder.Entity<TechnicianAssignment>(entity =>
        {
            entity.ToTable("assignments");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.TechnicianName).HasMaxLength(200).IsRequired();
            entity.HasIndex(item => item.IssueId).IsUnique();
            entity.HasOne<MaintenanceIssue>()
                .WithMany()
                .HasForeignKey(item => item.IssueId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasData(
                new TechnicianAssignment { Id = 1, IssueId = 1, TechnicianName = "Alex Martin" },
                new TechnicianAssignment { Id = 2, IssueId = 2, TechnicianName = "Sara Khan" }
            );
        });
    }
}