namespace HotelOS.Shared.Algorithms;

public sealed record RoomCandidate(
    int RoomId,
    string RoomNumber,
    bool IsClean,
    int Floor,
    int DistanceScore,
    int PriorityScore,
    bool MatchesPreference,
    string? RoomType = null,
    int GuestCapacity = 0);

public static class RoomAssignmentAlgorithm
{
    public static IReadOnlyList<RoomCandidate> RankAvailableRooms(IEnumerable<RoomCandidate> rooms)
        => rooms
            .Where(room => room.IsClean)
            .OrderByDescending(room => room.MatchesPreference)
            .ThenByDescending(room => room.PriorityScore)
            .ThenByDescending(room => room.GuestCapacity)
            .ThenBy(room => room.DistanceScore)
            .ThenBy(room => room.Floor)
            .ThenBy(room => room.RoomType ?? string.Empty)
            .ToList();
}