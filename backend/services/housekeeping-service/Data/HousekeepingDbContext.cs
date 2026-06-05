using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotelOS.HousekeepingService.Models;

namespace HotelOS.HousekeepingService.Data;

public sealed class HousekeepingDbContext(DbContextOptions<HousekeepingDbContext> options) : DbContext(options)
{
    public DbSet<CleaningTask> CleaningTasks => Set<CleaningTask>();
    public DbSet<RoomStatus> RoomStatuses => Set<RoomStatus>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}