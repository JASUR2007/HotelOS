using HotelOS.RoomService.Models;

namespace HotelOS.RoomService.Repositories;

public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Room> AddAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room> UpdateAsync(Room room, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Amenity>> GetAllAmenitiesAsync(CancellationToken cancellationToken = default);
    Task<List<Amenity>> GetAmenitiesByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}
