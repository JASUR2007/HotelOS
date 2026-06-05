namespace HotelOS.Shared.Algorithms;

public sealed record RoomCandidate(
    int RoomId,
    string RoomNumber,
    bool IsClean,
    int Floor,
    int DistanceScore,
    int PriorityScore,
    bool MatchesPreference);

public static class RoomAssignmentAlgorithm
{
    public static IReadOnlyList<RoomCandidate> RankAvailableRooms(IEnumerable<RoomCandidate> rooms)
        => rooms
            .Where(room => room.IsClean)
            .OrderByDescending(room => room.MatchesPreference)
            .ThenByDescending(room => room.PriorityScore)
            .ThenBy(room => room.DistanceScore)
            .ThenBy(room => room.Floor)
            .ToList();
}