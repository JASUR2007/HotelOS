using HotelOS.PaymentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelOS.PaymentService.Data;

public sealed class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<TransactionLog> TransactionLogs => Set<TransactionLog>();
    public DbSet<PaymentHistory> PaymentHistory => Set<PaymentHistory>();
    public DbSet<IdempotentRefund> IdempotentRefunds => Set<IdempotentRefund>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).ValueGeneratedOnAdd().UseIdentityColumn();
            entity.Property(item => item.InvoiceNumber).HasMaxLength(50).IsRequired();
            entity.Property(item => item.GuestName).HasMaxLength(200).IsRequired();
            entity.Property(item => item.RoomNumber).HasMaxLength(20).IsRequired();
            entity.Property(item => item.Status).HasMaxLength(50).IsRequired();
            entity.Property(item => item.TotalAmount).HasPrecision(12, 2);
            entity.HasIndex(item => item.InvoiceNumber).IsUnique();
            entity.HasData(new Invoice { Id = 1, InvoiceNumber = "INV-10021", GuestName = "Amelia Stone", RoomNumber = "101", TotalAmount = 420m, Status = "Paid" });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).ValueGeneratedOnAdd().UseIdentityColumn();
            entity.Property(item => item.Method).HasMaxLength(50).IsRequired();
            entity.Property(item => item.Status).HasMaxLength(50).IsRequired();
            entity.Property(item => item.Amount).HasPrecision(12, 2);
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Message).HasMaxLength(400).IsRequired();
        });

        modelBuilder.Entity<PaymentHistory>(entity =>
        {
            entity.ToTable("payment_history");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Status).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<IdempotentRefund>(entity =>
        {
            entity.ToTable("idempotent_refunds");
            entity.HasKey(item => item.IdempotencyKey);
            entity.Property(item => item.IdempotencyKey).HasMaxLength(200);
        });
    }
}