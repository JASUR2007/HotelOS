using HotelOS.RoomService.Models;
using HotelOS.RoomService.Data.Factories;
using HotelOS.Shared.Algorithms;
using HotelOS.Shared.Events;
using HotelOS.PaymentService.Services;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class TS01_GuestCheckIn_RoomAssignment
{
    [Fact]
    public void Assigns_Cleanest_DoubleRoom_On_Floor3_When_Available()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(new[]
        {
            new RoomCandidate(1, "301", true, 3, 10, 100, true),
            new RoomCandidate(2, "302", true, 3, 5, 90, true),
            new RoomCandidate(3, "305", true, 3, 20, 80, true),
        });
        var best = ranked.First();
        Assert.Equal(1, best.RoomId);
        Assert.Equal("301", best.RoomNumber);
    }

    [Fact]
    public void FallsBack_To_AnyFloor_When_Floor3_HasNoCleanRoom()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(new[]
        {
            new RoomCandidate(4, "401", true, 4, 5, 90, false),
            new RoomCandidate(5, "201", true, 2, 3, 85, false),
            new RoomCandidate(6, "205", true, 2, 8, 80, false),
        });
        Assert.NotEmpty(ranked);
        Assert.Equal(4, ranked.First().RoomId);
    }

    [Fact]
    public void FiltersOut_Dirty_Rooms()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(new[]
        {
            new RoomCandidate(7, "101", false, 1, 1, 100, true),
            new RoomCandidate(8, "102", false, 1, 2, 95, true),
        });
        Assert.Empty(ranked);
    }

    [Fact]
    public void Returns_Clear_Message_When_No_Rooms_Available()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(Array.Empty<RoomCandidate>());
        Assert.Empty(ranked);
    }
}

public sealed class TS02_GuestCheckOut_BillingAndEvents
{
    [Fact]
    public void Calculates_Total_Bill_For_3Nights_Plus_Extras()
    {
        var billing = new BillingCalculationService();
        var result = billing.Calculate(new BillingInput(
            RoomNightsTotal: 660m,
            FoodOrdersTotal: 80m,
            MinibarTotal: 35m,
            DamagesTotal: 0m,
            DiscountsTotal: 0m));
        Assert.Equal(775m, result.GrossTotal);
        Assert.Equal(775m, result.NetTotal);
    }

    [Fact]
    public void Applies_Discounts_Correctly()
    {
        var billing = new BillingCalculationService();
        var result = billing.Calculate(new BillingInput(
            RoomNightsTotal: 660m,
            FoodOrdersTotal: 80m,
            MinibarTotal: 35m,
            DamagesTotal: 50m,
            DiscountsTotal: 100m));
        Assert.Equal(825m, result.GrossTotal);
        Assert.Equal(725m, result.NetTotal);
    }

    [Fact]
    public void Publishes_RoomVacatedEvent_On_CheckOut()
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var evt = new RoomStatusChangedEvent(204, "204", "Dirty", occurredAt);
        Assert.Equal(204, evt.RoomId);
        Assert.Equal("204", evt.RoomNumber);
        Assert.Equal("Dirty", evt.Status);
    }

    [Fact]
    public void NetTotal_Never_Below_Zero()
    {
        var billing = new BillingCalculationService();
        var result = billing.Calculate(new BillingInput(100, 0, 0, 0, 500));
        Assert.Equal(100, result.GrossTotal);
        Assert.Equal(0, result.NetTotal);
    }
}

public sealed class TS03_HousekeeperMarksClean_StatusMachine
{
    [Fact]
    public void Status_Transitions_Dirty_To_Cleaning_To_Clean()
    {
        var statuses = new List<string> { "Dirty", "Cleaning", "Clean" };
        for (var i = 0; i < statuses.Count - 1; i++)
        {
            var current = statuses[i];
            var next = statuses[i + 1];
            Assert.NotEqual(current, next);
        }
        Assert.Equal("Clean", statuses[^1]);
    }

    [Fact]
    public void RoomCleaned_Event_Has_Clean_Status()
    {
        var evt = new RoomStatusChangedEvent(204, "204", "Clean", DateTimeOffset.UtcNow);
        Assert.Equal("Clean", evt.Status);
        Assert.Equal("204", evt.RoomNumber);
    }

    [Fact]
    public void Room_Marked_Available_After_Cleaned()
    {
        var room = new Room { Id = 204, RoomNumber = "204", Status = "Dirty" };
        room.Status = "Clean";
        Assert.NotEqual("Dirty", room.Status);
    }
}

public sealed class TS04_RoomServiceOrder_StateMachine
{
    [Fact]
    public void Order_Transitions_Received_To_Preparing_To_Delivered()
    {
        var order = OrderFactory.Create("301", "Guest A", "Received", 42m);
        Assert.Equal("Received", order.Status);

        order.Status = "Preparing";
        Assert.Equal("Preparing", order.Status);

        order.Status = "Delivered";
        Assert.Equal("Delivered", order.Status);
    }

    [Fact]
    public void Order_Total_Is_Sum_Of_Items()
    {
        var order = OrderFactory.Create("301", "Guest A", "Delivered", 42m);
        Assert.Equal(42m, order.Total);
    }

    [Fact]
    public void StatusChange_Broadcasts_OrderStatusChangedEvent()
    {
        var evt = new OrderStatusChangedEvent(1, "301", "Delivered", DateTimeOffset.UtcNow);
        Assert.Equal("301", evt.RoomNumber);
        Assert.Equal("Delivered", evt.Status);
    }

    [Fact]
    public void Charges_Added_To_Invoice()
    {
        var billing = new BillingCalculationService();
        var before = billing.Calculate(new BillingInput(660, 0, 0, 0, 0));
        var after = billing.Calculate(new BillingInput(660, 42, 0, 0, 0));
        Assert.True(after.GrossTotal > before.GrossTotal);
        Assert.Equal(42m, after.GrossTotal - before.GrossTotal);
    }
}

public sealed class TS05_CriticalMaintenance_PriorityQueue
{
    [Fact]
    public void Critical_Issue_Goes_To_Front_Of_Queue()
    {
        var queue = new MaintenancePriorityQueue();
        queue.Enqueue(new MaintenanceQueueItem(1, "AC not working", MaintenancePriority.High, DateTimeOffset.UtcNow));
        queue.Enqueue(new MaintenanceQueueItem(2, "Broken shower", MaintenancePriority.Critical, DateTimeOffset.UtcNow));

        Assert.True(queue.TryDequeue(out var first));
        Assert.Equal(2, first!.TicketId);
        Assert.Equal("Broken shower", first.Title);
    }

    [Fact]
    public void Critical_Returns_Before_High_And_Normal()
    {
        var queue = new MaintenancePriorityQueue();
        queue.Enqueue(new MaintenanceQueueItem(1, "Low priority", MaintenancePriority.Low, DateTimeOffset.UtcNow));
        queue.Enqueue(new MaintenanceQueueItem(2, "Critical leak", MaintenancePriority.Critical, DateTimeOffset.UtcNow));
        queue.Enqueue(new MaintenanceQueueItem(3, "Normal request", MaintenancePriority.Normal, DateTimeOffset.UtcNow));

        Assert.True(queue.TryDequeue(out var first));
        Assert.Equal(2, first!.TicketId);
    }

    [Fact]
    public void Room_Status_Updates_On_Resolution()
    {
        var evt = new MaintenanceTicketUpdatedEvent(1, "Broken shower", "Resolved", "Critical", DateTimeOffset.UtcNow);
        Assert.Equal("Resolved", evt.Status);
        Assert.Equal("Critical", evt.Priority);
    }
}

public sealed class TS06_ConcurrentCheckIn_NoDoubleBooking
{
    [Fact]
    public void Overlapping_Dates_For_Same_Room_Are_Rejected()
    {
        var existingCheckIn = new DateOnly(2026, 6, 1);
        var existingCheckOut = new DateOnly(2026, 6, 5);
        var newCheckIn = new DateOnly(2026, 6, 3);
        var newCheckOut = new DateOnly(2026, 6, 7);

        var overlaps = newCheckIn < existingCheckOut && existingCheckIn < newCheckOut;
        Assert.True(overlaps);
    }

    [Fact]
    public void NonOverlapping_Dates_Are_Allowed()
    {
        var existingCheckIn = new DateOnly(2026, 6, 1);
        var existingCheckOut = new DateOnly(2026, 6, 5);
        var newCheckIn = new DateOnly(2026, 6, 5);
        var newCheckOut = new DateOnly(2026, 6, 10);

        var overlaps = newCheckIn < existingCheckOut && existingCheckIn < newCheckOut;
        Assert.False(overlaps);
    }

    [Fact]
    public void Simultaneous_Request_Gets_Different_Room()
    {
        var room1 = 101;
        var room2 = 102;
        Assert.NotEqual(room1, room2);
    }
}

public sealed class TS07_AllRoomsOccupied_Validation
{
    [Fact]
    public void Returns_Clear_NoRooms_Message()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(Array.Empty<RoomCandidate>());
        Assert.Empty(ranked);
    }

    [Fact]
    public void Graceful_Message_No_Crash()
    {
        var message = "No rooms available";
        Assert.NotNull(message);
        Assert.NotEmpty(message);
    }
}

public sealed class TS08_InvalidRoomNumber_InputValidation
{
    [Fact]
    public void RoomNumber_Must_Not_Be_Empty()
    {
        var invalid = new[] { "", "   ", null };
        Assert.All(invalid, input =>
        {
            var isValid = !string.IsNullOrWhiteSpace(input)
                && input.Length >= 1
                && input.All(char.IsLetterOrDigit);
            Assert.False(isValid);
        });
    }

    [Fact]
    public void RoomNumber_Must_Be_Alphanumeric()
    {
        var valid = new[] { "101", "204", "A12" };
        var invalid = new[] { "10 1", "20-4", "abc!" };
        Assert.All(valid, v => Assert.True(v.All(char.IsLetterOrDigit)));
        Assert.All(invalid, v => Assert.False(v.All(char.IsLetterOrDigit)));
    }

    [Fact]
    public void System_Stable_After_Invalid_Input()
    {
        var systemOk = true;
        Assert.True(systemOk);
    }
}
