using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.ReceptionService.Repositories;

public sealed class GuestRepository(ReceptionDbContext context) : IGuestRepository
{
    public async Task<IReadOnlyList<Guest>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Guests.AsNoTracking().OrderByDescending(g => g.Id).ToListAsync(cancellationToken);

    public Task<Guest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => context.Guests.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public Task<Guest?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => context.Guests.FirstOrDefaultAsync(g => g.Email.ToLower() == email.ToLower(), cancellationToken);

    public async Task<Guest> AddAsync(Guest guest, CancellationToken cancellationToken = default)
    {
        context.Guests.Add(guest);
        await context.SaveChangesAsync(cancellationToken);
        return guest;
    }

    public async Task UpdateAsync(Guest guest, CancellationToken cancellationToken = default)
    {
        context.Entry(guest).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var guest = await context.Guests.FindAsync([id], cancellationToken)
            ?? throw new InvalidOperationException("Guest not found");
        context.Guests.Remove(guest);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
