using HotelOS.RoomService.DTOs;

namespace HotelOS.RoomService.Services;

public interface IRoomCommands
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto request, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto request, CancellationToken cancellationToken = default);
    Task<RoomDto> CreateRoomAsync(CreateRoomDto request, CancellationToken cancellationToken = default);
    Task<RoomDto> UpdateRoomAsync(int id, UpdateRoomDto request, CancellationToken cancellationToken = default);
    Task<RoomDto> PatchRoomStatusAsync(int id, PatchRoomStatusDto request, CancellationToken cancellationToken = default);
    Task DeleteRoomAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default);
}
