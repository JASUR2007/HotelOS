using HotelOS.UserService.DTOs;
using HotelOS.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IUserService service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request, CancellationToken cancellationToken)
        => Ok(await service.LoginAsync(request, cancellationToken));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request, CancellationToken cancellationToken)
        => Ok(await service.RegisterAsync(request, cancellationToken));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto request, CancellationToken cancellationToken)
        => Ok(await service.RefreshAsync(request, cancellationToken));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto request, CancellationToken cancellationToken)
    {
        await service.LogoutAsync(request, cancellationToken);
        return NoContent();
    }
}