using HotelOS.HousekeepingService.Models;

namespace HotelOS.HousekeepingService.Repositories;

public interface IHousekeepingRepository
{
    Task<IReadOnlyList<CleaningTask>> GetQueueAsync(CancellationToken cancellationToken = default);
    Task<CleaningTask?> GetByIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<CleaningTask> AddAsync(CleaningTask task, CancellationToken cancellationToken = default);
    Task DeleteAsync(CleaningTask task, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}