using HotelOS.WebsocketService.Models;

namespace HotelOS.WebsocketService.Repositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<NotificationRecord>> GetAllAsync(string? role = null, CancellationToken cancellationToken = default);
    Task<NotificationRecord> AddAsync(NotificationRecord record, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default);
}