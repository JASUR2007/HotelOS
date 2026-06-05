using HotelOS.UserService.DTOs;
using HotelOS.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IUserService service) : ControllerBase
{
    /// <summary>Authenticates a user with email and password, returning JWT tokens.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "email": "john@example.com",
    ///   "password": "securePassword123"
    /// }
    /// Response example:
    /// {
    ///   "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    ///   "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    ///   "expiresIn": 3600,
    ///   "user": {
    ///     "id": 1,
    ///     "email": "john@example.com",
    ///     "displayName": "John Doe",
    ///     "status": "Active",
    ///     "roles": ["Admin"],
    ///     "permissions": ["users:read", "users:write"]
    ///   }
    /// }
    /// </remarks>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request, CancellationToken cancellationToken)
        => Ok(await service.LoginAsync(request, cancellationToken));

    /// <summary>Registers a new user account and returns JWT tokens.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "email": "johndoe@example.com",
    ///   "displayName": "John Doe",
    ///   "password": "securePassword123"
    /// }
    /// Response example:
    /// {
    ///   "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    ///   "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    ///   "expiresIn": 3600,
    ///   "user": {
    ///     "id": 2,
    ///     "email": "johndoe@example.com",
    ///     "displayName": "John Doe",
    ///     "status": "Active",
    ///     "roles": [],
    ///     "permissions": []
    ///   }
    /// }
    /// </remarks>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request, CancellationToken cancellationToken)
        => Ok(await service.RegisterAsync(request, cancellationToken));

    /// <summary>Refreshes an expired access token using a valid refresh token.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
    /// }
    /// Response example:
    /// {
    ///   "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    ///   "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    ///   "expiresIn": 3600,
    ///   "user": {
    ///     "id": 1,
    ///     "email": "john@example.com",
    ///     "displayName": "John Doe",
    ///     "status": "Active",
    ///     "roles": ["Admin"],
    ///     "permissions": ["users:read"]
    ///   }
    /// }
    /// </remarks>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto request, CancellationToken cancellationToken)
        => Ok(await service.RefreshAsync(request, cancellationToken));

    /// <summary>Invalidates the provided refresh token, ending the user session.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
    /// }
    /// Response: 204 No Content
    /// </remarks>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto request, CancellationToken cancellationToken)
    {
        await service.LogoutAsync(request, cancellationToken);
        return NoContent();
    }
}