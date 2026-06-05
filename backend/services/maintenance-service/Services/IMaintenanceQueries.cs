using HotelOS.MaintenanceService.DTOs;

namespace HotelOS.MaintenanceService.Services;

public interface IMaintenanceQueries
{
    Task<IReadOnlyList<MaintenanceIssueDto>> GetIssuesAsync(CancellationToken cancellationToken = default);
}
