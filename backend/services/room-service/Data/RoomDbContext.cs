using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Data;

public sealed class RoomDbContext(DbContextOptions<RoomDbContext> options) : DbContext(options)
{
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<FoodOrder> FoodOrders => Set<FoodOrder>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<RoomKey> RoomKeys => Set<RoomKey>();
    public DbSet<MasterKey> MasterKeys => Set<MasterKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("room_service");

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("menu_items");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(150).IsRequired();
            entity.Property(item => item.Price).HasPrecision(10, 2);
            entity.HasData(
                new MenuItem { Id = 1, Name = "Breakfast set", Price = 18m },
                new MenuItem { Id = 2, Name = "Club sandwich", Price = 14m },
                new MenuItem { Id = 3, Name = "Sparkling water", Price = 4m }
            );
        });

        modelBuilder.Entity<FoodOrder>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.RoomNumber).HasMaxLength(20).IsRequired();
            entity.Property(item => item.GuestName).HasMaxLength(200).IsRequired();
            entity.Property(item => item.Status).HasMaxLength(50).IsRequired();
            entity.Property(item => item.Total).HasPrecision(10, 2);
            entity.HasIndex(item => new { item.RoomNumber, item.Status });
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("rooms");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RoomNumber).HasMaxLength(20).IsRequired();
            entity.HasIndex(r => r.RoomNumber).IsUnique();
            entity.Property(r => r.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(r => r.Status).HasMaxLength(50).IsRequired();
            entity.Property(r => r.PricePerNight).HasPrecision(10, 2);
            entity.Property(r => r.Description).HasMaxLength(2000);
            entity.Property(r => r.MainImage).HasMaxLength(500);
            entity.Property(r => r.Images).HasColumnType("jsonb");
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.BranchId);
            entity.HasMany(r => r.Amenities)
                .WithMany(a => a.Rooms)
                .UsingEntity<Dictionary<string, object>>(
                    "room_amenities",
                    j => j.HasOne<Amenity>().WithMany().HasForeignKey("amenity_id"),
                    j => j.HasOne<Room>().WithMany().HasForeignKey("room_id"));
        });

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.ToTable("amenities");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(a => a.Name).IsUnique();
            entity.Property(a => a.IconUrl).HasMaxLength(300);
            entity.Property(a => a.Description).HasMaxLength(500);
            entity.HasData(
                new Amenity { Id = 1, Name = "WiFi", IconUrl = "/images/amenities/wifi.svg", Description = "High-speed internet access" },
                new Amenity { Id = 2, Name = "Coffee Machine", IconUrl = "/images/amenities/coffee.svg", Description = "Coffee machine with complimentary pods" },
                new Amenity { Id = 3, Name = "Air Conditioning", IconUrl = "/images/amenities/ac.svg", Description = "Climate control system" },
                new Amenity { Id = 4, Name = "Smart TV", IconUrl = "/images/amenities/tv.svg", Description = "Smart TV with streaming services" },
                new Amenity { Id = 5, Name = "TV", IconUrl = "/images/amenities/tv.svg", Description = "Flat-screen television" },
                new Amenity { Id = 6, Name = "Mini Bar", IconUrl = "/images/amenities/minibar.svg", Description = "Stocked mini bar" },
                new Amenity { Id = 7, Name = "Bath", IconUrl = "/images/amenities/bath.svg", Description = "Private bathroom" },
                new Amenity { Id = 8, Name = "Jacuzzi", IconUrl = "/images/amenities/jacuzzi.svg", Description = "Whirlpool bathtub" },
                new Amenity { Id = 9, Name = "Balcony", IconUrl = "/images/amenities/balcony.svg", Description = "Private balcony with view" },
                new Amenity { Id = 10, Name = "Wheelchair Access", IconUrl = "/images/amenities/wheelchair.svg", Description = "Wheelchair accessible" },
                new Amenity { Id = 11, Name = "Roll-in Shower", IconUrl = "/images/amenities/shower.svg", Description = "Accessible roll-in shower" },
                new Amenity { Id = 12, Name = "Butler Service", IconUrl = "/images/amenities/butler.svg", Description = "Dedicated butler service" }
            );
        });

        modelBuilder.Entity<RoomKey>(entity =>
        {
            entity.ToTable("room_keys");
            entity.HasKey(k => k.Id);
            entity.Property(k => k.RoomNumber).HasMaxLength(20).IsRequired();
            entity.Property(k => k.KeyType).HasMaxLength(20).IsRequired();
            entity.Property(k => k.Status).HasMaxLength(20).IsRequired();
            entity.Property(k => k.IssuedTo).HasMaxLength(200);
            entity.Property(k => k.IssuedBy).HasMaxLength(200);
            entity.Property(k => k.CreatedAt).IsRequired();
            entity.HasIndex(k => k.BranchId);
            entity.HasIndex(k => k.RoomId);
            entity.HasIndex(k => k.Status);
        });

        modelBuilder.Entity<MasterKey>(entity =>
        {
            entity.ToTable("master_keys");
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Name).HasMaxLength(200).IsRequired();
            entity.Property(k => k.Description).HasMaxLength(500);
            entity.Property(k => k.AccessScope).HasMaxLength(100).IsRequired();
            entity.Property(k => k.Status).HasMaxLength(20).IsRequired();
            entity.Property(k => k.IssuedTo).HasMaxLength(200);
            entity.Property(k => k.CreatedAt).IsRequired();
            entity.HasIndex(k => k.Name);
        });
    }
}
