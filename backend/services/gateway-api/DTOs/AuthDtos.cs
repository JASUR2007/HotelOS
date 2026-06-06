namespace HotelOS.GatewayApi.DTOs;

public sealed record LoginRequestDto(string Email, string Password);

public sealed record RefreshTokenRequestDto(string RefreshToken);

public sealed record AuthPermissionDto(int Id, string Name, string Description);

public sealed record AuthRoleDto(int Id, string Name, string[] Permissions);

public sealed record AuthUserDto(int Id, string Email, string DisplayName, string Role, string[] Permissions);

public sealed record LoginResponseDto(string AccessToken, string RefreshToken, int ExpiresIn, AuthUserDto User);

public sealed record RefreshTokenResponseDto(string AccessToken, string RefreshToken, int ExpiresIn);