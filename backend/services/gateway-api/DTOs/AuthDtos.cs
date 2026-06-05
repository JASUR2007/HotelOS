namespace HotelOS.GatewayApi.DTOs;

/// <summary>Request body for user login.</summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <example>
/// {
///   "email": "admin@hotelos.com",
///   "password": "admin123"
/// }
/// </example>
public sealed record LoginRequestDto(string Email, string Password);

/// <summary>Request body for refreshing an access token.</summary>
/// <param name="RefreshToken">A valid refresh token issued at login.</param>
/// <example>
/// {
///   "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
/// }
/// </example>
public sealed record RefreshTokenRequestDto(string RefreshToken);

/// <summary>Represents a single permission available in the system.</summary>
/// <example>
/// {
///   "id": 1,
///   "name": "view_dashboard",
///   "description": "Allows viewing the hotel dashboard"
/// }
/// </example>
public sealed record AuthPermissionDto(int Id, string Name, string Description);

/// <summary>Represents a role with its assigned permissions.</summary>
/// <example>
/// {
///   "id": 1,
///   "name": "Admin",
///   "permissions": ["view_dashboard", "manage_bookings", "view_reports"]
/// }
/// </example>
public sealed record AuthRoleDto(int Id, string Name, string[] Permissions);

/// <summary>Represents an authenticated user.</summary>
/// <example>
/// {
///   "id": 1,
///   "email": "admin@hotelos.com",
///   "displayName": "Admin User",
///   "role": "Admin",
///   "permissions": ["view_dashboard", "manage_bookings", "view_reports"]
/// }
/// </example>
public sealed record AuthUserDto(int Id, string Email, string DisplayName, string Role, string[] Permissions);

/// <summary>Response returned after successful login.</summary>
/// <example>
/// {
///   "accessToken": "eyJhbGciOiJIUzI1NiIs...",
///   "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
///   "expiresIn": 3600,
///   "user": {
///     "id": 1,
///     "email": "admin@hotelos.com",
///     "displayName": "Admin User",
///     "role": "Admin",
///     "permissions": ["view_dashboard", "manage_bookings"]
///   }
/// }
/// </example>
public sealed record LoginResponseDto(string AccessToken, string RefreshToken, int ExpiresIn, AuthUserDto User);

/// <summary>Response returned after refreshing an access token.</summary>
/// <example>
/// {
///   "accessToken": "eyJhbGciOiJIUzI1NiIs...",
///   "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
///   "expiresIn": 3600
/// }
/// </example>
public sealed record RefreshTokenResponseDto(string AccessToken, string RefreshToken, int ExpiresIn);