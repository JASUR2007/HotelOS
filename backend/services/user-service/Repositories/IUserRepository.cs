using HotelOS.UserService.DTOs;
using HotelOS.UserService.Models;

namespace HotelOS.UserService.Repositories;

public interface IUserRepository
{
    Task<UserDto?> FindByCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<UserDto?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(RegisterUserDto request, CancellationToken cancellationToken = default);
    Task StoreRefreshTokenAsync(int userId, string token, DateTimeOffset expiresAt, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(RoleAssignmentDto request, CancellationToken cancellationToken = default);
    Task AssignPermissionAsync(PermissionAssignmentDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppRole>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppPermission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
    Task UpdateUserAsync(int userId, UpdateUserDto request, CancellationToken cancellationToken = default);
    Task<AppRole> CreateRoleAsync(CreateRoleDto request, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(int roleId, UpdateRoleDto request, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default);
    Task RemoveRolePermissionAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task RemoveRolePermissionByNameAsync(int roleId, string permissionName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDetailDto>> GetAllRolesWithPermissionsAsync(CancellationToken cancellationToken = default);
    Task<AppPermission> CreatePermissionAsync(CreatePermissionDto request, CancellationToken cancellationToken = default);
    Task UpdatePermissionAsync(int permissionId, UpdatePermissionDto request, CancellationToken cancellationToken = default);
    Task DeletePermissionAsync(int permissionId, CancellationToken cancellationToken = default);
}