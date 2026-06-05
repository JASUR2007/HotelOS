using HotelOS.UserService.DTOs;

namespace HotelOS.UserService.Services;

public interface IUserService
{
    Task<AuthResponseDto> LoginAsync(LoginUserDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshAsync(RefreshTokenDto request, CancellationToken cancellationToken = default);
    Task LogoutAsync(RefreshTokenDto request, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(RoleAssignmentDto request, CancellationToken cancellationToken = default);
    Task AssignPermissionAsync(PermissionAssignmentDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task UpdateUserAsync(string id, UpdateUserDto request, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto request, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(int roleId, UpdateRoleDto request, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default);
    Task RemoveRolePermissionAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task RemoveRolePermissionByNameAsync(int roleId, string permissionName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDetailDto>> GetRolesDetailedAsync(CancellationToken cancellationToken = default);
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto request, CancellationToken cancellationToken = default);
    Task UpdatePermissionAsync(int permissionId, UpdatePermissionDto request, CancellationToken cancellationToken = default);
    Task DeletePermissionAsync(int permissionId, CancellationToken cancellationToken = default);
}