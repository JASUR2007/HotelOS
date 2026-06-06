using HotelOS.Shared.Algorithms;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class TS03_MaintenancePriorityQueueTests
{
    [Fact]
    public void Dequeue_Returns_Critical_Before_High()
    {
        var queue = new MaintenancePriorityQueue();
        queue.Enqueue(new MaintenanceQueueItem(1, "High", MaintenancePriority.High, DateTimeOffset.UtcNow));
        queue.Enqueue(new MaintenanceQueueItem(2, "Critical", MaintenancePriority.Critical, DateTimeOffset.UtcNow));
        queue.Enqueue(new MaintenanceQueueItem(3, "Normal", MaintenancePriority.Normal, DateTimeOffset.UtcNow));

        Assert.True(queue.TryDequeue(out var first));
        Assert.Equal(2, first!.TicketId);

        Assert.True(queue.TryDequeue(out var second));
        Assert.Equal(1, second!.TicketId);

        Assert.True(queue.TryDequeue(out var third));
        Assert.Equal(3, third!.TicketId);
    }

    [Fact]
    public void TryDequeue_Returns_False_When_Empty()
    {
        var queue = new MaintenancePriorityQueue();
        Assert.False(queue.TryDequeue(out _));
    }

    [Fact]
    public void Enqueue_Added_Ticket_Is_Dequeued()
    {
        var queue = new MaintenancePriorityQueue();
        queue.Enqueue(new MaintenanceQueueItem(5, "Test", MaintenancePriority.Low, DateTimeOffset.UtcNow));

        Assert.True(queue.TryDequeue(out var item));
        Assert.Equal(5, item!.TicketId);
        Assert.False(queue.TryDequeue(out _));
    }

    [Fact]
    public void TryDequeue_Preserves_Fifo_Order_For_Same_Priority()
    {
        var queue = new MaintenancePriorityQueue();
        var now = DateTimeOffset.UtcNow;
        queue.Enqueue(new MaintenanceQueueItem(1, "First issue", MaintenancePriority.High, now));
        queue.Enqueue(new MaintenanceQueueItem(2, "Second issue", MaintenancePriority.High, now.AddSeconds(1)));

        Assert.True(queue.TryDequeue(out var first));
        Assert.Equal(1, first!.TicketId);
        Assert.True(queue.TryDequeue(out var second));
        Assert.Equal(2, second!.TicketId);
    }
}
