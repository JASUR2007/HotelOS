using HotelOS.WebsocketService.DTOs;

namespace HotelOS.WebsocketService.Services;

public interface INotificationBroadcastService
{
    Task BroadcastAsync(RealtimeNotificationDto notification, CancellationToken cancellationToken = default);
    Task BroadcastAsync(RealtimeNotificationDto notification, string methodName, object? eventData, CancellationToken cancellationToken = default);
}
