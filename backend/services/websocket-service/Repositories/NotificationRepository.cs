using HotelOS.WebsocketService.Data;
using HotelOS.WebsocketService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.WebsocketService.Repositories;

public sealed class NotificationRepository(WebsocketDbContext context) : INotificationRepository
{
    public async Task<IReadOnlyList<NotificationRecord>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Notifications.AsNoTracking().OrderByDescending(item => item.Id).ToListAsync(cancellationToken);

    public async Task<NotificationRecord> AddAsync(NotificationRecord record, CancellationToken cancellationToken = default)
    {
        context.Notifications.Add(record);
        await context.SaveChangesAsync(cancellationToken);
        return record;
    }
}