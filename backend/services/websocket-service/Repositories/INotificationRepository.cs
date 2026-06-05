using HotelOS.WebsocketService.Models;

namespace HotelOS.WebsocketService.Repositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<NotificationRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<NotificationRecord> AddAsync(NotificationRecord record, CancellationToken cancellationToken = default);
}