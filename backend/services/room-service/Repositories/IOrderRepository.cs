using HotelOS.RoomService.Models;

namespace HotelOS.RoomService.Repositories;

public interface IOrderRepository
{
    Task<FoodOrder> AddAsync(FoodOrder order, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FoodOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
