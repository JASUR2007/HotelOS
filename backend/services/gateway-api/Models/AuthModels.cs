namespace HotelOS.GatewayApi.Models;

public sealed class ApplicationUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class ApplicationRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class ApplicationPermission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

public sealed class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}

public sealed class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}