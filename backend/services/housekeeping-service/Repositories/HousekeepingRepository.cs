using HotelOS.HousekeepingService.Data;
using HotelOS.HousekeepingService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.HousekeepingService.Repositories;

public sealed class HousekeepingRepository(HousekeepingDbContext context) : IHousekeepingRepository
{
    public async Task<IReadOnlyList<CleaningTask>> GetQueueAsync(CancellationToken cancellationToken = default)
        => await context.CleaningTasks.AsNoTracking().ToListAsync(cancellationToken);

    public Task<CleaningTask?> GetByIdAsync(int taskId, CancellationToken cancellationToken = default)
        => context.CleaningTasks.FirstOrDefaultAsync(task => task.Id == taskId, cancellationToken);

    public async Task<CleaningTask> AddAsync(CleaningTask task, CancellationToken cancellationToken = default)
    {
        context.CleaningTasks.Add(task);
        await context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public Task DeleteAsync(CleaningTask task, CancellationToken cancellationToken = default)
    {
        context.CleaningTasks.Remove(task);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}