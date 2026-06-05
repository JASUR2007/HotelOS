using HotelOS.Shared.Algorithms;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class MaintenancePriorityQueueTests
{
    [Fact]
    public void Dequeue_ReturnsCriticalTicketFirst()
    {
        var queue = new MaintenancePriorityQueue();
        queue.Enqueue(new MaintenanceQueueItem(2, "Normal", MaintenancePriority.Normal, DateTimeOffset.UtcNow));
        queue.Enqueue(new MaintenanceQueueItem(1, "Critical", MaintenancePriority.Critical, DateTimeOffset.UtcNow));

        Assert.True(queue.TryDequeue(out var first));
        Assert.Equal(1, first!.TicketId);
    }
}