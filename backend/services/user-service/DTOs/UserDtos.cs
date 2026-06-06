namespace HotelOS.UserService.DTOs;

public sealed record RegisterUserDto(string Email, string DisplayName, string Password);
public sealed record LoginUserDto(string Email, string Password);
public sealed record RefreshTokenDto(string RefreshToken);
public sealed record RoleAssignmentDto(int UserId, int RoleId);
public sealed record PermissionAssignmentDto(int RoleId, int PermissionId = 0, string? PermissionName = null);
public sealed record CreateUserDto(string Email, string DisplayName, string Password, string Role);
public sealed record UserDto(int Id, string Email, string DisplayName, string Status, string[] Roles, string[] Permissions);
public sealed record AuthResponseDto(string AccessToken, string RefreshToken, int ExpiresIn, UserDto User);
public sealed record RoleDto(int Id, string Name);
public sealed record PermissionDto(int Id, string Name, string Description);
public sealed record UpdateUserDto(string DisplayName);
public sealed record CreateRoleDto(string Name, int[]? PermissionIds = null, string[]? PermissionNames = null);
public sealed record RoleDetailDto(int Id, string Name, string[] Permissions);
public sealed record UpdateRoleDto(string Name);
public sealed record CreatePermissionDto(string Name, string Description);
public sealed record UpdatePermissionDto(string Name, string Description);