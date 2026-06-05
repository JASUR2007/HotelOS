using HotelOS.Shared.Algorithms;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class TS02_RoomAssignmentAlgorithmTests
{
    [Fact]
    public void RankAvailableRooms_FiltersOut_DirtyRooms()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(new[]
        {
            new RoomCandidate(1, "101", true, 1, 1, 50, false),
            new RoomCandidate(2, "102", false, 1, 2, 90, true),
            new RoomCandidate(3, "103", true, 2, 3, 30, true),
        });

        Assert.Equal(2, ranked.Count);
        Assert.DoesNotContain(ranked, r => r.RoomId == 2);
    }

    [Fact]
    public void RankAvailableRooms_Prefers_MatchingPreference()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(new[]
        {
            new RoomCandidate(1, "101", true, 1, 1, 90, false),
            new RoomCandidate(2, "102", true, 1, 2, 50, true),
        });

        Assert.Equal(2, ranked[0].RoomId);
    }

    [Fact]
    public void RankAvailableRooms_Sorts_By_Priority_When_Preference_Equal()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(new[]
        {
            new RoomCandidate(1, "101", true, 1, 5, 30, false),
            new RoomCandidate(2, "102", true, 1, 5, 80, false),
        });

        Assert.Equal(2, ranked[0].RoomId);
    }

    [Fact]
    public void RankAvailableRooms_Returns_Empty_When_No_Clean_Rooms()
    {
        var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(new[]
        {
            new RoomCandidate(1, "101", false, 1, 1, 100, true),
        });

        Assert.Empty(ranked);
    }
}
