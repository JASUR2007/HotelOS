namespace HotelOS.Shared.Algorithms;

public enum MaintenancePriority
{
    Critical = 0,
    High = 1,
    Normal = 2,
    Low = 3
}

public sealed record MaintenanceQueueItem(int TicketId, string Title, MaintenancePriority Priority, DateTimeOffset CreatedAt);

public sealed class MaintenancePriorityQueue
{
    private readonly PriorityQueue<MaintenanceQueueItem, int> queue = new();

    public void Enqueue(MaintenanceQueueItem item) => queue.Enqueue(item, (int)item.Priority);

    public bool TryDequeue(out MaintenanceQueueItem? item)
    {
        var success = queue.TryDequeue(out var value, out _);
        item = value;
        return success;
    }
}