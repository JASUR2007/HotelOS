using HotelOS.WebsocketService.Data;
using HotelOS.WebsocketService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.WebsocketService.Repositories;

public sealed class NotificationRepository(WebsocketDbContext context) : INotificationRepository
{
    public async Task<IReadOnlyList<NotificationRecord>> GetAllAsync(string? role = null, CancellationToken cancellationToken = default)
    {
        IQueryable<NotificationRecord> query = context.Notifications.AsNoTracking();

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(n => n.TargetRole == null || n.TargetRole == role);
        }

        return await query.OrderByDescending(item => item.Id).ToListAsync(cancellationToken);
    }

    public async Task<NotificationRecord> AddAsync(NotificationRecord record, CancellationToken cancellationToken = default)
    {
        context.Notifications.Add(record);
        await context.SaveChangesAsync(cancellationToken);
        return record;
    }

    public async Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var notification = await context.Notifications.FindAsync([id], cancellationToken);
        if (notification is not null)
        {
            notification.IsRead = true;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}