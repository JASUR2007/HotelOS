using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace HotelOS.WebsocketService.Hubs;

public sealed class NotificationsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var role = Context.User?.FindFirst("role")?.Value
                ?? Context.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        if (!string.IsNullOrEmpty(role))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, role);
        }
        await base.OnConnectedAsync();
    }
}

public interface INotificationClient
{
    Task NotificationReceived(object notification);
}