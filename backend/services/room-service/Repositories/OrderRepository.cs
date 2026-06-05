using HotelOS.RoomService.Data;
using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Repositories;

public sealed class OrderRepository(RoomDbContext context) : IOrderRepository
{
    public async Task<FoodOrder> AddAsync(FoodOrder order, CancellationToken cancellationToken = default)
    {
        context.FoodOrders.Add(order);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<IReadOnlyList<FoodOrder>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.FoodOrders.AsNoTracking().ToListAsync(cancellationToken);

    public Task<FoodOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => context.FoodOrders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await context.FoodOrders.FindAsync([id], cancellationToken)
            ?? throw new InvalidOperationException("Order not found");
        context.FoodOrders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);
    }
}
