using HotelOS.MaintenanceService.Models;

namespace HotelOS.MaintenanceService.Repositories;

public interface IMaintenanceRepository
{
    Task<IReadOnlyList<MaintenanceIssue>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MaintenanceIssue?> GetByIdAsync(int issueId, CancellationToken cancellationToken = default);
    Task<MaintenanceIssue> AddAsync(MaintenanceIssue issue, CancellationToken cancellationToken = default);
    Task DeleteAsync(MaintenanceIssue issue, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}