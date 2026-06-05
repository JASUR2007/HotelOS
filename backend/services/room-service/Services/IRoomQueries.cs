using HotelOS.RoomService.DTOs;
using HotelOS.Shared.Algorithms;

namespace HotelOS.RoomService.Services;

public interface IRoomQueries
{
    Task<IReadOnlyList<MenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoomDto>> GetRoomsAsync(CancellationToken cancellationToken = default);
    Task<RoomDto> GetRoomByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoomOverviewDto>> GetRoomsOverviewAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoomCandidate>> GetAvailableRoomCandidatesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AmenityDto>> GetAmenitiesAsync(CancellationToken cancellationToken = default);
}
