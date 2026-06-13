using HotelOS.RoomService.Data;
using HotelOS.RoomService.DTOs;
using HotelOS.RoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Services;

public sealed class KeyService(RoomDbContext context) : IKeyQueries, IKeyCommands
{
    public async Task<IReadOnlyList<RoomKeyDto>> GetKeysAsync(CancellationToken cancellationToken = default)
        => await context.Set<RoomKey>()
            .OrderBy(k => k.RoomNumber)
            .Select(k => new RoomKeyDto(k.Id, k.BranchId, k.RoomId, k.RoomNumber, k.KeyType, k.Status, k.IssuedTo, k.IssuedBy,
                k.IssuedAt.HasValue ? k.IssuedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                k.ReturnedAt.HasValue ? k.ReturnedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                k.CreatedAt.ToString("yyyy-MM-dd HH:mm")))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RoomKeyDto>> GetKeysByRoomAsync(int roomId, CancellationToken cancellationToken = default)
        => await context.Set<RoomKey>()
            .Where(k => k.RoomId == roomId)
            .Select(k => new RoomKeyDto(k.Id, k.BranchId, k.RoomId, k.RoomNumber, k.KeyType, k.Status, k.IssuedTo, k.IssuedBy,
                k.IssuedAt.HasValue ? k.IssuedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                k.ReturnedAt.HasValue ? k.ReturnedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                k.CreatedAt.ToString("yyyy-MM-dd HH:mm")))
            .ToListAsync(cancellationToken);

    public async Task<RoomKeyDto> IssueKeyAsync(IssueKeyDto request, CancellationToken cancellationToken = default)
    {
        var existing = await context.Set<RoomKey>()
            .FirstOrDefaultAsync(k => k.RoomId == request.RoomId && k.Status == "Available", cancellationToken);

        RoomKey key;
        if (existing is not null)
        {
            key = existing;
        }
        else
        {
            key = new RoomKey
            {
                BranchId = request.BranchId,
                RoomId = request.RoomId,
                RoomNumber = request.RoomNumber,
                KeyType = "Room",
                Status = "Available",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Set<RoomKey>().Add(key);
        }

        key.Status = "Issued";
        key.IssuedTo = request.IssuedTo;
        key.IssuedBy = request.IssuedBy;
        key.IssuedAt = DateTimeOffset.UtcNow;
        key.ReturnedAt = null;
        await context.SaveChangesAsync(cancellationToken);

        return new RoomKeyDto(key.Id, key.BranchId, key.RoomId, key.RoomNumber, key.KeyType, key.Status, key.IssuedTo, key.IssuedBy,
            key.IssuedAt.Value.ToString("yyyy-MM-dd HH:mm"), null, key.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
    }

    public async Task<RoomKeyDto> ReturnKeyAsync(int keyId, CancellationToken cancellationToken = default)
    {
        var key = await context.Set<RoomKey>().FindAsync([keyId], cancellationToken)
            ?? throw new InvalidOperationException("Key not found");

        key.Status = "Returned";
        key.ReturnedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return new RoomKeyDto(key.Id, key.BranchId, key.RoomId, key.RoomNumber, key.KeyType, key.Status, key.IssuedTo, key.IssuedBy,
            key.IssuedAt?.ToString("yyyy-MM-dd HH:mm"), key.ReturnedAt?.ToString("yyyy-MM-dd HH:mm"), key.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
    }

    public async Task<RoomKeyDto> MarkKeyLostAsync(int keyId, CancellationToken cancellationToken = default)
    {
        var key = await context.Set<RoomKey>().FindAsync([keyId], cancellationToken)
            ?? throw new InvalidOperationException("Key not found");

        key.Status = "Lost";
        await context.SaveChangesAsync(cancellationToken);

        return new RoomKeyDto(key.Id, key.BranchId, key.RoomId, key.RoomNumber, key.KeyType, key.Status, key.IssuedTo, key.IssuedBy,
            key.IssuedAt?.ToString("yyyy-MM-dd HH:mm"), key.ReturnedAt?.ToString("yyyy-MM-dd HH:mm"), key.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
    }

    public async Task<MasterKeyDto> CreateMasterKeyAsync(CreateMasterKeyDto request, CancellationToken cancellationToken = default)
    {
        var key = new MasterKey
        {
            Name = request.Name,
            Description = request.Description,
            AccessScope = request.AccessScope,
            Status = "Available",
            CreatedAt = DateTimeOffset.UtcNow
        };

        context.Set<MasterKey>().Add(key);
        await context.SaveChangesAsync(cancellationToken);

        return new MasterKeyDto(key.Id, key.Name, key.Description, key.AccessScope, key.Status, null, null, null, key.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
    }

    public async Task<IReadOnlyList<MasterKeyDto>> GetMasterKeysAsync(CancellationToken cancellationToken = default)
        => await context.Set<MasterKey>()
            .OrderBy(k => k.Name)
            .Select(k => new MasterKeyDto(k.Id, k.Name, k.Description, k.AccessScope, k.Status, k.IssuedTo,
                k.IssuedAt.HasValue ? k.IssuedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                k.ReturnedAt.HasValue ? k.ReturnedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                k.CreatedAt.ToString("yyyy-MM-dd HH:mm")))
            .ToListAsync(cancellationToken);

    public async Task<MasterKeyDto> IssueMasterKeyAsync(int id, IssueMasterKeyDto request, CancellationToken cancellationToken = default)
    {
        var key = await context.Set<MasterKey>().FindAsync([id], cancellationToken)
            ?? throw new InvalidOperationException("Master key not found");

        key.Status = "Issued";
        key.IssuedTo = request.IssuedTo;
        key.IssuedAt = DateTimeOffset.UtcNow;
        key.ReturnedAt = null;
        await context.SaveChangesAsync(cancellationToken);

        return new MasterKeyDto(key.Id, key.Name, key.Description, key.AccessScope, key.Status, key.IssuedTo,
            key.IssuedAt?.ToString("yyyy-MM-dd HH:mm"), null, key.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
    }

    public async Task<MasterKeyDto> ReturnMasterKeyAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = await context.Set<MasterKey>().FindAsync([id], cancellationToken)
            ?? throw new InvalidOperationException("Master key not found");

        key.Status = "Available";
        key.ReturnedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return new MasterKeyDto(key.Id, key.Name, key.Description, key.AccessScope, key.Status, key.IssuedTo,
            key.IssuedAt?.ToString("yyyy-MM-dd HH:mm"), key.ReturnedAt?.ToString("yyyy-MM-dd HH:mm"), key.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
    }

    public async Task DeleteKeyAsync(int keyId, CancellationToken cancellationToken = default)
    {
        var key = await context.Set<RoomKey>().FindAsync([keyId], cancellationToken)
            ?? throw new InvalidOperationException("Key not found");
        context.Set<RoomKey>().Remove(key);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMasterKeyAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = await context.Set<MasterKey>().FindAsync([id], cancellationToken)
            ?? throw new InvalidOperationException("Master key not found");
        context.Set<MasterKey>().Remove(key);
        await context.SaveChangesAsync(cancellationToken);
    }
}
