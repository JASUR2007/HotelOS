namespace HotelOS.Shared.Constants;

public static class RabbitMqQueues
{
    public const string Events = "hotelos.events";
    public const string EventsDlq = "hotelos.events.dlq";
    public const string EventsRetry = "hotelos.events.retry";
    public const string Notifications = "hotelos.websocket.events";

    public const string TopicExchange = "hotelos.topic";
}

public static class RabbitMqRoutingKeys
{
    public const string BookingCreated = "booking.created";
    public const string BookingCancelled = "booking.cancelled";
    public const string GuestCheckedIn = "guest.checked_in";
    public const string GuestCheckedOut = "guest.checked_out";
    public const string RoomVacated = "room.vacated";
    public const string RoomCleaned = "room.cleaned";
    public const string RoomOccupied = "room.occupied";
    public const string OrderCreated = "order.created";
    public const string OrderPreparing = "order.preparing";
    public const string OrderDelivered = "order.delivered";
    public const string MaintenanceCreated = "maintenance.created";
    public const string MaintenanceAssigned = "maintenance.assigned";
    public const string MaintenanceCompleted = "maintenance.completed";
    public const string PaymentCompleted = "payment.completed";
    public const string PaymentRefunded = "payment.refunded";
}
