using HotelOS.ReceptionService.Models;

namespace HotelOS.ReceptionService.Repositories;

public interface IGuestRepository
{
    Task<IReadOnlyList<Guest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Guest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Guest?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Guest> AddAsync(Guest guest, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guest guest, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
