using HotelOS.Shared.Algorithms;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class RoomAssignmentAlgorithmTests
{
    [Fact]
    public void RankAvailableRooms_PrefersCleanPreferenceAndPriority()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(
        [
            new RoomCandidate(2, "302", true, 3, 4, 50, false),
            new RoomCandidate(1, "101", true, 1, 1, 90, true),
            new RoomCandidate(3, "204", false, 2, 2, 100, true)
        ]);

        Assert.Equal(new[] { 1, 2 }, ranked.Select(item => item.RoomId));
    }
}