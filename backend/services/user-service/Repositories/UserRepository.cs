using HotelOS.UserService.Data;
using HotelOS.UserService.DTOs;
using HotelOS.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.UserService.Repositories;

public sealed class UserRepository(UserDbContext context) : IUserRepository
{
    public async Task<UserDto?> FindByCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(item => item.Email == email && item.PasswordHash == password, cancellationToken);
        return user is null ? null : await MapUserAsync(user.Id, cancellationToken);
    }

    public async Task<UserDto?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(item => item.Token == refreshToken && item.RevokedAt == null && item.ExpiresAt > DateTimeOffset.UtcNow, cancellationToken);
        return token is null ? null : await MapUserAsync(token.UserId, cancellationToken);
    }

    public async Task<UserDto> CreateUserAsync(RegisterUserDto request, CancellationToken cancellationToken = default)
    {
        var user = new AppUser { Email = request.Email, DisplayName = request.DisplayName, PasswordHash = request.Password, Status = "Active" };
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return await MapUserAsync(user.Id, cancellationToken);
    }

    public async Task StoreRefreshTokenAsync(int userId, string token, DateTimeOffset expiresAt, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Add(new RefreshToken { UserId = userId, Token = token, ExpiresAt = expiresAt });
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignRoleAsync(RoleAssignmentDto request, CancellationToken cancellationToken = default)
    {
        context.UserRoles.Add(new UserRoleBridge { UserId = request.UserId, RoleId = request.RoleId });
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignPermissionAsync(PermissionAssignmentDto request, CancellationToken cancellationToken = default)
    {
        var permissionId = request.PermissionId;
        if (permissionId == 0 && !string.IsNullOrEmpty(request.PermissionName))
        {
            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == request.PermissionName, cancellationToken)
                ?? throw new InvalidOperationException($"Permission '{request.PermissionName}' not found");
            permissionId = permission.Id;
        }
        if (permissionId == 0)
            throw new InvalidOperationException("Permission ID or name must be provided");

        var exists = await context.RolePermissions.AnyAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == permissionId, cancellationToken);
        if (!exists)
        {
            context.RolePermissions.Add(new RolePermissionBridge { RoleId = request.RoleId, PermissionId = permissionId });
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.Users.AsNoTracking().ToListAsync(cancellationToken);
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            result.Add(await MapUserAsync(user.Id, cancellationToken));
        }
        return result;
    }

    public async Task DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        if (int.TryParse(id, out var userId))
        {
            var user = await context.Users.FindAsync([userId], cancellationToken);
            if (user is not null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public Task<IReadOnlyList<AppRole>> GetAllRolesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AppRole>>(context.Roles.AsNoTracking().ToList());

    public Task<IReadOnlyList<AppPermission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AppPermission>>(context.Permissions.AsNoTracking().ToList());

    public async Task UpdateUserAsync(int userId, UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([userId], cancellationToken)
            ?? throw new InvalidOperationException("User not found");
        user.DisplayName = request.DisplayName;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AppRole> CreateRoleAsync(CreateRoleDto request, CancellationToken cancellationToken = default)
    {
        var role = new AppRole { Name = request.Name };
        context.Roles.Add(role);
        await context.SaveChangesAsync(cancellationToken);

        var permissionIds = new List<int>();
        if (request.PermissionIds is { Length: > 0 })
            permissionIds.AddRange(request.PermissionIds);
        if (request.PermissionNames is { Length: > 0 })
        {
            var idsFromNames = await context.Permissions
                .Where(p => request.PermissionNames.Contains(p.Name))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);
            permissionIds.AddRange(idsFromNames);
        }

        if (permissionIds.Count > 0)
        {
            foreach (var permissionId in permissionIds.Distinct())
            {
                context.RolePermissions.Add(new RolePermissionBridge { RoleId = role.Id, PermissionId = permissionId });
            }
            await context.SaveChangesAsync(cancellationToken);
        }

        return role;
    }

    public async Task UpdateRoleAsync(int roleId, UpdateRoleDto request, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.FindAsync([roleId], cancellationToken)
            ?? throw new InvalidOperationException("Role not found");
        role.Name = request.Name;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.FindAsync([roleId], cancellationToken)
            ?? throw new InvalidOperationException("Role not found");
        context.Roles.Remove(role);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRolePermissionAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        var bridge = await context.RolePermissions.FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken)
            ?? throw new InvalidOperationException("Role-permission association not found");
        context.RolePermissions.Remove(bridge);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRolePermissionByNameAsync(int roleId, string permissionName, CancellationToken cancellationToken = default)
    {
        var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == permissionName, cancellationToken)
            ?? throw new InvalidOperationException($"Permission '{permissionName}' not found");
        await RemoveRolePermissionAsync(roleId, permission.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<RoleDetailDto>> GetAllRolesWithPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles.AsNoTracking().ToListAsync(cancellationToken);
        var result = new List<RoleDetailDto>();
        foreach (var role in roles)
        {
            var permissions = await (
                from bridge in context.RolePermissions.AsNoTracking()
                join permission in context.Permissions.AsNoTracking() on bridge.PermissionId equals permission.Id
                where bridge.RoleId == role.Id
                select permission.Name
            ).ToArrayAsync(cancellationToken);
            result.Add(new RoleDetailDto(role.Id, role.Name, permissions));
        }
        return result;
    }

    public async Task<AppPermission> CreatePermissionAsync(CreatePermissionDto request, CancellationToken cancellationToken = default)
    {
        var permission = new AppPermission { Name = request.Name, Description = request.Description };
        context.Permissions.Add(permission);
        await context.SaveChangesAsync(cancellationToken);
        return permission;
    }

    public async Task UpdatePermissionAsync(int permissionId, UpdatePermissionDto request, CancellationToken cancellationToken = default)
    {
        var permission = await context.Permissions.FindAsync([permissionId], cancellationToken)
            ?? throw new InvalidOperationException("Permission not found");
        permission.Name = request.Name;
        permission.Description = request.Description;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePermissionAsync(int permissionId, CancellationToken cancellationToken = default)
    {
        var permission = await context.Permissions.FindAsync([permissionId], cancellationToken)
            ?? throw new InvalidOperationException("Permission not found");
        context.Permissions.Remove(permission);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<UserDto> MapUserAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking().FirstAsync(item => item.Id == userId, cancellationToken);
        var roleNames = await (
            from bridge in context.UserRoles.AsNoTracking()
            join role in context.Roles.AsNoTracking() on bridge.RoleId equals role.Id
            where bridge.UserId == userId
            select role.Name
        ).ToArrayAsync(cancellationToken);

        var permissions = await (
            from userRole in context.UserRoles.AsNoTracking()
            join bridge in context.RolePermissions.AsNoTracking() on userRole.RoleId equals bridge.RoleId
            join permission in context.Permissions.AsNoTracking() on bridge.PermissionId equals permission.Id
            where userRole.UserId == userId
            select permission.Name
        ).Distinct().ToArrayAsync(cancellationToken);

        return new UserDto(user.Id, user.Email, user.DisplayName, user.Status, roleNames, permissions);
    }
}