using HotelOS.HousekeepingService.DTOs;

namespace HotelOS.HousekeepingService.Services;

public interface IHousekeepingService
{
    Task<IReadOnlyList<CleaningTaskDto>> GetQueueAsync(CancellationToken cancellationToken = default);
    Task<CleaningTaskDto> UpdateStatusAsync(UpdateCleaningStatusDto request, CancellationToken cancellationToken = default);
    Task<CleaningTaskDto> CreateTaskAsync(CreateCleaningTaskDto request, CancellationToken cancellationToken = default);
    Task<CleaningTaskDto> UpdateTaskAsync(int id, UpdateCleaningTaskDto request, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
}