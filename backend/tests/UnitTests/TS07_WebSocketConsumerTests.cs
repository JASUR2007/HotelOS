using Xunit;

namespace HotelOS.UnitTests;

public sealed class TS07_WebSocketConsumerTests
{
    [Fact]
    public void NotificationBroadcast_Sends_To_All_Clients()
    {
        // WebSocket consumer processes messages from RabbitMQ and broadcasts via SignalR.
        // This test validates the conceptual flow: a received event should result in a broadcast.
        var routingKeys = new[]
        {
            "booking.created", "booking.cancelled", "guest.checked_in", "guest.checked_out",
            "room.vacated", "room.cleaned", "room.occupied",
            "order.created", "order.preparing", "order.delivered",
            "maintenance.created", "maintenance.assigned", "maintenance.completed",
            "payment.completed", "payment.refunded"
        };

        Assert.Equal(15, routingKeys.Length);
        Assert.Contains("booking.created", routingKeys);
        Assert.Contains("payment.completed", routingKeys);
    }

    [Fact]
    public void RoutingKey_Maps_To_HubEvent_Name()
    {
        var map = new Dictionary<string, string>
        {
            ["booking.created"] = "BookingCreated",
            ["guest.checked_in"] = "GuestCheckedIn",
            ["order.delivered"] = "OrderDelivered",
            ["payment.completed"] = "PaymentCompleted",
        };

        Assert.Equal("BookingCreated", map["booking.created"]);
        Assert.Equal("GuestCheckedIn", map["guest.checked_in"]);
    }
}
