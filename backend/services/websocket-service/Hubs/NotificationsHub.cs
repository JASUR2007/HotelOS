using Microsoft.AspNetCore.SignalR;

namespace HotelOS.WebsocketService.Hubs;

public sealed class NotificationsHub : Hub
{
}

public interface INotificationClient
{
    Task NotificationReceived(object notification);
}