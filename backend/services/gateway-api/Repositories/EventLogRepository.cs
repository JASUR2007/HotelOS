using HotelOS.GatewayApi.Data;
using HotelOS.GatewayApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.GatewayApi.Repositories;

public interface IEventLogRepository
{
    Task AppendAsync(GatewayAuditLog entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GatewayAuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
}

public sealed class EventLogRepository(GatewayDbContext context) : IEventLogRepository
{
    public async Task AppendAsync(GatewayAuditLog entry, CancellationToken cancellationToken = default)
    {
        context.AuditLogs.Add(entry);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GatewayAuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.AuditLogs.AsNoTracking().OrderByDescending(item => item.CreatedAt).ToListAsync(cancellationToken);
}