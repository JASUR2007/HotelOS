using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HotelOS.UserService.DTOs;
using HotelOS.UserService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HotelOS.UserService.Services;

public sealed class UserService(IUserRepository repository, IConfiguration configuration) : IUserService
{
    public async Task<AuthResponseDto> LoginAsync(LoginUserDto request, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindByCredentialsAsync(request.Email, request.Password, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        return await BuildAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto request, CancellationToken cancellationToken = default)
    {
        var user = await repository.CreateUserAsync(request, cancellationToken);
        return await BuildAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenDto request, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindByRefreshTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        return await BuildAuthResponseAsync(user, cancellationToken, request.RefreshToken);
    }

    public Task LogoutAsync(RefreshTokenDto request, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task AssignRoleAsync(RoleAssignmentDto request, CancellationToken cancellationToken = default)
        => repository.AssignRoleAsync(request, cancellationToken);

    public Task AssignPermissionAsync(PermissionAssignmentDto request, CancellationToken cancellationToken = default)
        => repository.AssignPermissionAsync(request, cancellationToken);

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
        => await repository.GetAllAsync(cancellationToken);

    public async Task<UserDto> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        var userDto = await repository.CreateUserAsync(new RegisterUserDto(request.Email, request.DisplayName, request.Password), cancellationToken);
        if (!string.IsNullOrEmpty(request.Role))
        {
            var roles = await repository.GetAllRolesAsync(cancellationToken);
            var role = roles.FirstOrDefault(r =>
                r.Name.Equals(request.Role, StringComparison.OrdinalIgnoreCase));
            if (role is not null)
            {
                await repository.AssignRoleAsync(new RoleAssignmentDto(userDto.Id, role.Id), cancellationToken);
            }
        }
        return userDto;
    }

    public async Task DeleteUserAsync(string id, CancellationToken cancellationToken = default)
        => await repository.DeleteUserAsync(id, cancellationToken);

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllRolesAsync(cancellationToken)).Select(r => new RoleDto(r.Id, r.Name)).ToList();

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllPermissionsAsync(cancellationToken)).Select(p => new PermissionDto(p.Id, p.Name, p.Description)).ToList();

    public async Task UpdateUserAsync(string id, UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(id, out var userId))
            throw new InvalidOperationException("Invalid user id");
        await repository.UpdateUserAsync(userId, request, cancellationToken);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto request, CancellationToken cancellationToken = default)
    {
        var role = await repository.CreateRoleAsync(request, cancellationToken);
        return new RoleDto(role.Id, role.Name);
    }

    public Task UpdateRoleAsync(int roleId, UpdateRoleDto request, CancellationToken cancellationToken = default)
        => repository.UpdateRoleAsync(roleId, request, cancellationToken);

    public Task DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default)
        => repository.DeleteRoleAsync(roleId, cancellationToken);

    public Task RemoveRolePermissionAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
        => repository.RemoveRolePermissionAsync(roleId, permissionId, cancellationToken);

    public Task RemoveRolePermissionByNameAsync(int roleId, string permissionName, CancellationToken cancellationToken = default)
        => repository.RemoveRolePermissionByNameAsync(roleId, permissionName, cancellationToken);

    public async Task<IReadOnlyList<RoleDetailDto>> GetRolesDetailedAsync(CancellationToken cancellationToken = default)
        => await repository.GetAllRolesWithPermissionsAsync(cancellationToken);

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto request, CancellationToken cancellationToken = default)
    {
        var permission = await repository.CreatePermissionAsync(request, cancellationToken);
        return new PermissionDto(permission.Id, permission.Name, permission.Description);
    }

    public Task UpdatePermissionAsync(int permissionId, UpdatePermissionDto request, CancellationToken cancellationToken = default)
        => repository.UpdatePermissionAsync(permissionId, request, cancellationToken);

    public Task DeletePermissionAsync(int permissionId, CancellationToken cancellationToken = default)
        => repository.DeletePermissionAsync(permissionId, cancellationToken);

    private async Task<AuthResponseDto> BuildAuthResponseAsync(UserDto user, CancellationToken cancellationToken, string? refreshToken = null)
    {
        var accessToken = CreateJwt(user);
        var token = refreshToken ?? CreateRefreshToken();
        await repository.StoreRefreshTokenAsync(user.Id, token, DateTimeOffset.UtcNow.AddDays(7), cancellationToken);
        return new AuthResponseDto(accessToken, token, 3600, user);
    }

    private string CreateJwt(UserDto user)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key missing");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Permissions.Select(permission => new Claim("permission", permission)));

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}