namespace HotelOS.UserService.Models;

public sealed class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}

public sealed class AppRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class AppPermission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class UserRoleBridge
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

public sealed class RolePermissionBridge
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

public sealed class LoginHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTimeOffset LoggedInAt { get; set; }
}

public sealed class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}