using HotelOS.MaintenanceService.Data;
using HotelOS.MaintenanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.MaintenanceService.Repositories;

public sealed class MaintenanceRepository(MaintenanceDbContext context) : IMaintenanceRepository
{
    public async Task<IReadOnlyList<MaintenanceIssue>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.MaintenanceIssues.AsNoTracking().ToListAsync(cancellationToken);

    public Task<MaintenanceIssue?> GetByIdAsync(int issueId, CancellationToken cancellationToken = default)
        => context.MaintenanceIssues.FirstOrDefaultAsync(issue => issue.Id == issueId, cancellationToken);

    public async Task<MaintenanceIssue> AddAsync(MaintenanceIssue issue, CancellationToken cancellationToken = default)
    {
        context.MaintenanceIssues.Add(issue);
        await context.SaveChangesAsync(cancellationToken);
        return issue;
    }

    public Task DeleteAsync(MaintenanceIssue issue, CancellationToken cancellationToken = default)
    {
        context.MaintenanceIssues.Remove(issue);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}