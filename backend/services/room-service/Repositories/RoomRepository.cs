using HotelOS.RoomService.Data;
using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Repositories;

public sealed class RoomRepository(RoomDbContext context) : IRoomRepository
{
    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Rooms.AsNoTracking().Include(r => r.Amenities).OrderBy(r => r.RoomNumber).ToListAsync(cancellationToken);

    public Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => context.Rooms.Include(r => r.Amenities).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        context.Rooms.Add(room);
        await context.SaveChangesAsync(cancellationToken);
        return room;
    }

    public async Task<Room> UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        context.Rooms.Update(room);
        await context.SaveChangesAsync(cancellationToken);
        return room;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var room = await context.Rooms.FindAsync([id], cancellationToken);
        if (room is not null)
        {
            context.Rooms.Remove(room);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<IReadOnlyList<Amenity>> GetAllAmenitiesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Amenity>>(context.Amenities.AsNoTracking().OrderBy(a => a.Name).ToList());

    public async Task<List<Amenity>> GetAmenitiesByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
    {
        if (ids is null || ids.Length == 0) return new List<Amenity>();
        var normalized = ids
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .ToArray();
        var intIds = normalized
            .Select(id => int.TryParse(id, out var value) ? value : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();
        var names = normalized
            .Where(id => !int.TryParse(id, out _))
            .ToList();
        return await context.Amenities
            .Where(a => intIds.Contains(a.Id) || names.Contains(a.Name))
            .ToListAsync(cancellationToken);
    }
}
