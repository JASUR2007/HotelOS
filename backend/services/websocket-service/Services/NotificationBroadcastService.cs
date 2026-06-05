using HotelOS.WebsocketService.DTOs;
using HotelOS.WebsocketService.Hubs;
using HotelOS.WebsocketService.Models;
using HotelOS.WebsocketService.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace HotelOS.WebsocketService.Services;

public sealed class NotificationBroadcastService(
    IHubContext<NotificationsHub> hubContext,
    INotificationRepository repository) : INotificationBroadcastService
{
    public async Task BroadcastAsync(
        RealtimeNotificationDto notification,
        string methodName = "NotificationReceived",
        object? eventData = null,
        CancellationToken cancellationToken = default)
    {
        await repository.AddAsync(new NotificationRecord
        {
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await hubContext.Clients.All.SendAsync("NotificationReceived", notification, cancellationToken);

        if (methodName != "NotificationReceived")
        {
            await hubContext.Clients.All.SendAsync(methodName, eventData ?? notification, cancellationToken);
        }
    }

    public Task BroadcastAsync(RealtimeNotificationDto notification, CancellationToken cancellationToken = default)
        => BroadcastAsync(notification, "NotificationReceived", null, cancellationToken);
}
