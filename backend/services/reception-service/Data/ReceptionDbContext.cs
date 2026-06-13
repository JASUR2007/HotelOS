using HotelOS.ReceptionService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelOS.ReceptionService.Data;

public sealed class ReceptionDbContext(DbContextOptions<ReceptionDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reception");

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.ToTable("guests");
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.Email).IsUnique();
            entity.Property(item => item.FullName).HasMaxLength(200).IsRequired();
            entity.Property(item => item.Email).HasMaxLength(200).IsRequired();
            entity.HasData(
                new Guest { Id = 1, FullName = "Amelia Stone", Email = "amelia.stone@hotelos.local" },
                new Guest { Id = 2, FullName = "Daniel Reed", Email = "daniel.reed@hotelos.local" }
            );
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("bookings");
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => new { item.GuestId, item.RoomId });
            entity.HasIndex(item => item.BranchId);
            entity.Property(item => item.Status).HasMaxLength(50).IsRequired();
            entity.Property(item => item.RowVersion).IsConcurrencyToken();
            entity.HasOne<Guest>()
                .WithMany()
                .HasForeignKey(item => item.GuestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasData(
                new Booking { Id = 1, BranchId = 1, GuestId = 1, RoomId = 101, CheckInDate = new DateOnly(2026, 5, 20), CheckOutDate = new DateOnly(2026, 5, 23), Status = "CheckedIn" },
                new Booking { Id = 2, BranchId = 1, GuestId = 2, RoomId = 207, CheckInDate = new DateOnly(2026, 5, 19), CheckOutDate = new DateOnly(2026, 5, 24), Status = "Booked" }
            );
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Currency).HasMaxLength(10).IsRequired();
            entity.HasIndex(item => item.BookingId);
            entity.HasOne<Booking>()
                .WithMany()
                .HasForeignKey(item => item.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasData(
                new Invoice { Id = 1, BookingId = 1, Total = 420m, Currency = "USD" },
                new Invoice { Id = 2, BookingId = 2, Total = 180m, Currency = "USD" }
            );
        });
    }
}