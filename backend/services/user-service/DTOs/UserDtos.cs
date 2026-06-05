namespace HotelOS.UserService.DTOs;

/// <summary>Data transfer object for user registration.</summary>
/// <param name="Email">User email address. <example>john@example.com</example></param>
/// <param name="DisplayName">User display name. <example>John Doe</example></param>
/// <param name="Password">User password. <example>securePassword123</example></param>
public sealed record RegisterUserDto(string Email, string DisplayName, string Password);
/// <summary>Data transfer object for user login.</summary>
/// <param name="Email">User email address. <example>john@example.com</example></param>
/// <param name="Password">User password. <example>securePassword123</example></param>
public sealed record LoginUserDto(string Email, string Password);
/// <summary>Data transfer object for token refresh.</summary>
/// <param name="RefreshToken">JWT refresh token. <example>eyJhbGciOiJIUzI1NiIs...</example></param>
public sealed record RefreshTokenDto(string RefreshToken);
/// <summary>Data transfer object for assigning a role to a user.</summary>
/// <param name="UserId">User identifier. <example>1</example></param>
/// <param name="RoleId">Role identifier. <example>2</example></param>
public sealed record RoleAssignmentDto(int UserId, int RoleId);
/// <summary>Data transfer object for assigning a permission to a role.</summary>
/// <param name="RoleId">Role identifier. <example>2</example></param>
/// <param name="PermissionId">Permission identifier. <example>5</example></param>
/// <param name="PermissionName">Permission name (alternative to PermissionId). <example>"view_dashboard"</example></param>
public sealed record PermissionAssignmentDto(int RoleId, int PermissionId = 0, string? PermissionName = null);
/// <summary>Data transfer object for admin creating a new user.</summary>
/// <param name="Email">User email address. <example>admin@hotelos.com</example></param>
/// <param name="DisplayName">User display name. <example>John Doe</example></param>
/// <param name="Password">User password. <example>securePassword123</example></param>
/// <param name="Role">User role name. <example>Receptionist</example></param>
public sealed record CreateUserDto(string Email, string DisplayName, string Password, string Role);
/// <summary>Data transfer object representing a user.</summary>
/// <param name="Id">User unique identifier. <example>1</example></param>
/// <param name="Email">User email address. <example>john@example.com</example></param>
/// <param name="DisplayName">User display name. <example>John Doe</example></param>
/// <param name="Status">User account status. <example>Active</example></param>
/// <param name="Roles">Roles assigned to the user. <example>["Admin", "Receptionist"]</example></param>
/// <param name="Permissions">Permissions assigned to the user. <example>["users:read", "users:write"]</example></param>
public sealed record UserDto(int Id, string Email, string DisplayName, string Status, string[] Roles, string[] Permissions);
/// <summary>Data transfer object for authentication response containing tokens and user info.</summary>
/// <param name="AccessToken">JWT access token. <example>eyJhbGciOiJIUzI1NiIs...</example></param>
/// <param name="RefreshToken">JWT refresh token. <example>eyJhbGciOiJIUzI1NiIs...</example></param>
/// <param name="ExpiresIn">Token expiration time in seconds. <example>3600</example></param>
/// <param name="User">Authenticated user details.</param>
public sealed record AuthResponseDto(string AccessToken, string RefreshToken, int ExpiresIn, UserDto User);
/// <summary>Data transfer object representing a role.</summary>
/// <param name="Id">Role identifier. <example>1</example></param>
/// <param name="Name">Role name. <example>Admin</example></param>
public sealed record RoleDto(int Id, string Name);
/// <summary>Data transfer object representing a permission.</summary>
/// <param name="Id">Permission identifier. <example>1</example></param>
/// <param name="Name">Permission name. <example>users:read</example></param>
/// <param name="Description">Permission description. <example>Allows reading user information</example></param>
public sealed record PermissionDto(int Id, string Name, string Description);
/// <summary>Data transfer object for updating a user's display name.</summary>
/// <param name="DisplayName">New display name. <example>John Doe</example></param>
public sealed record UpdateUserDto(string DisplayName);
/// <summary>Data transfer object for creating a new role with optional permissions.</summary>
/// <param name="Name">Role name. <example>Housekeeping</example></param>
/// <param name="PermissionIds">Optional list of permission IDs to assign. <example>[1, 2, 3]</example></param>
/// <param name="PermissionNames">Optional list of permission names to assign. <example>["view_dashboard", "manage_rooms"]</example></param>
public sealed record CreateRoleDto(string Name, int[]? PermissionIds = null, string[]? PermissionNames = null);
/// <summary>Data transfer object representing a role with its permissions.</summary>
/// <param name="Id">Role identifier. <example>1</example></param>
/// <param name="Name">Role name. <example>Admin</example></param>
/// <param name="Permissions">Permission names assigned to the role. <example>["view_dashboard", "manage_rooms"]</example></param>
public sealed record RoleDetailDto(int Id, string Name, string[] Permissions);
/// <summary>Data transfer object for updating a role's name.</summary>
/// <param name="Name">New role name. <example>Senior Housekeeping</example></param>
public sealed record UpdateRoleDto(string Name);
/// <summary>Data transfer object for creating a new permission.</summary>
/// <param name="Name">Permission name. <example>reports:read</example></param>
/// <param name="Description">Permission description. <example>Allows reading reports</example></param>
public sealed record CreatePermissionDto(string Name, string Description);
/// <summary>Data transfer object for updating a permission.</summary>
/// <param name="Name">New permission name. <example>reports:write</example></param>
/// <param name="Description">New permission description. <example>Allows creating and editing reports</example></param>
public sealed record UpdatePermissionDto(string Name, string Description);