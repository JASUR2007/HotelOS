using HotelOS.MaintenanceService.DTOs;

namespace HotelOS.MaintenanceService.Services;

public interface IMaintenanceService
{
    Task<IReadOnlyList<MaintenanceIssueDto>> GetIssuesAsync(CancellationToken cancellationToken = default);
    Task<MaintenanceIssueDto> CreateIssueAsync(CreateIssueDto request, CancellationToken cancellationToken = default);
    Task<MaintenanceIssueDto> AssignTechnicianAsync(AssignTechnicianDto request, CancellationToken cancellationToken = default);
    Task<MaintenanceIssueDto> UpdateIssueAsync(int id, UpdateIssueDto request, CancellationToken cancellationToken = default);
    Task DeleteIssueAsync(int id, CancellationToken cancellationToken = default);
}