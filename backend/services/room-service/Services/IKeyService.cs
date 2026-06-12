using HotelOS.RoomService.DTOs;

namespace HotelOS.RoomService.Services;

public interface IKeyQueries
{
    Task<IReadOnlyList<RoomKeyDto>> GetKeysAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoomKeyDto>> GetKeysByRoomAsync(int roomId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MasterKeyDto>> GetMasterKeysAsync(CancellationToken cancellationToken = default);
}

public interface IKeyCommands
{
    Task<RoomKeyDto> IssueKeyAsync(IssueKeyDto request, CancellationToken cancellationToken = default);
    Task<RoomKeyDto> ReturnKeyAsync(int keyId, CancellationToken cancellationToken = default);
    Task<RoomKeyDto> MarkKeyLostAsync(int keyId, CancellationToken cancellationToken = default);
    Task<MasterKeyDto> CreateMasterKeyAsync(CreateMasterKeyDto request, CancellationToken cancellationToken = default);
    Task<MasterKeyDto> IssueMasterKeyAsync(int id, IssueMasterKeyDto request, CancellationToken cancellationToken = default);
    Task<MasterKeyDto> ReturnMasterKeyAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteKeyAsync(int keyId, CancellationToken cancellationToken = default);
    Task DeleteMasterKeyAsync(int id, CancellationToken cancellationToken = default);
}
