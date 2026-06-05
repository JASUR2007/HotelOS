namespace HotelOS.GatewayApi.DTOs;

/// <param name="Email">User email address.</param>
/// <param name="Password">User password.</param>
public sealed record LoginRequestDto(string Email, string Password);

/// <param name="RefreshToken">Valid refresh token issued at login.</param>
public sealed record RefreshTokenRequestDto(string RefreshToken);

/// <summary>System permission.</summary>
public sealed record AuthPermissionDto(int Id, string Name, string Description);

/// <summary>Role with assigned permissions.</summary>
public sealed record AuthRoleDto(int Id, string Name, string[] Permissions);

/// <summary>Authenticated user.</summary>
public sealed record AuthUserDto(int Id, string Email, string DisplayName, string Role, string[] Permissions);

/// <summary>Successful login response.</summary>
public sealed record LoginResponseDto(string AccessToken, string RefreshToken, int ExpiresIn, AuthUserDto User);

/// <summary>Token refresh response.</summary>
public sealed record RefreshTokenResponseDto(string AccessToken, string RefreshToken, int ExpiresIn);