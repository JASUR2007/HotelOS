using HotelOS.UserService.DTOs;
using HotelOS.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AdminController(IUserService service) : ControllerBase
{
    /// <summary>Retrieves all registered users.</summary>
    /// <remarks>
    /// Response example:
    /// [
    ///   {
    ///     "id": 1,
    ///     "email": "admin@hotelos.com",
    ///     "displayName": "Admin User",
    ///     "status": "Active",
    ///     "roles": ["Admin"],
    ///     "permissions": ["users:read", "users:write"]
    ///   },
    ///   {
    ///     "id": 2,
    ///     "email": "johndoe@example.com",
    ///     "displayName": "John Doe",
    ///     "status": "Active",
    ///     "roles": ["Receptionist"],
    ///     "permissions": ["reservations:read"]
    ///   }
    /// ]
    /// </remarks>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        => Ok(await service.GetUsersAsync(cancellationToken));

    /// <summary>Creates a new user account with a specified role.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "email": "newuser@example.com",
    ///   "displayName": "New User",
    ///   "password": "securePassword123",
    ///   "role": "Housekeeping"
    /// }
    /// Response example:
    /// {
    ///   "id": 3,
    ///   "email": "newuser@example.com",
    ///   "displayName": "New User",
    ///   "status": "Active",
    ///   "roles": ["Housekeeping"],
    ///   "permissions": []
    /// }
    /// </remarks>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request, CancellationToken cancellationToken)
        => Ok(await service.CreateUserAsync(request, cancellationToken));

    /// <summary>Deletes a user by their unique identifier.</summary>
    /// <remarks>
    /// Response: 204 No Content
    /// </remarks>
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        await service.DeleteUserAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Assigns a role to a user.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "userId": 1,
    ///   "roleId": 3
    /// }
    /// Response: 200 OK
    /// </remarks>
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentDto request, CancellationToken cancellationToken)
    {
        await service.AssignRoleAsync(request, cancellationToken);
        return Ok();
    }

    /// <summary>Assigns a permission to a role.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "roleId": 2,
    ///   "permissionId": 5
    /// }
    /// Response: 200 OK
    /// </remarks>
    [HttpPost("assign-permission")]
    public async Task<IActionResult> AssignPermission([FromBody] PermissionAssignmentDto request, CancellationToken cancellationToken)
    {
        await service.AssignPermissionAsync(request, cancellationToken);
        return Ok();
    }

    /// <summary>Retrieves all roles with their assigned permissions.</summary>
    /// <remarks>
    /// Response example:
    /// [
    ///   { "id": 1, "name": "Admin", "permissions": ["view_dashboard", "manage_users"] },
    ///   { "id": 2, "name": "Receptionist", "permissions": ["create_booking"] }
    /// ]
    /// </remarks>
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        => Ok(await service.GetRolesDetailedAsync(cancellationToken));

    /// <summary>Retrieves all permissions.</summary>
    /// <remarks>
    /// Response example:
    /// [
    ///   { "id": 1, "name": "users:read", "description": "Allows reading user information" },
    ///   { "id": 2, "name": "users:write", "description": "Allows creating and editing users" },
    ///   { "id": 3, "name": "reservations:read", "description": "Allows reading reservations" }
    /// ]
    /// </remarks>
    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
        => Ok(await service.GetPermissionsAsync(cancellationToken));

    /// <summary>Updates a user's display name.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "displayName": "John Updated"
    /// }
    /// Response: 200 OK
    /// </remarks>
    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto request, CancellationToken cancellationToken)
    {
        await service.UpdateUserAsync(id, request, cancellationToken);
        return Ok();
    }

    /// <summary>Creates a new role with optional permission assignments.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "name": "Housekeeping",
    ///   "permissionIds": [1, 3]
    /// }
    /// Response example:
    /// {
    ///   "id": 5,
    ///   "name": "Housekeeping"
    /// }
    /// </remarks>
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto request, CancellationToken cancellationToken)
        => Ok(await service.CreateRoleAsync(request, cancellationToken));

    /// <summary>Updates a role's name.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "name": "Senior Housekeeping"
    /// }
    /// Response: 200 OK
    /// </remarks>
    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto request, CancellationToken cancellationToken)
    {
        await service.UpdateRoleAsync(id, request, cancellationToken);
        return Ok();
    }

    /// <summary>Deletes a role by its identifier.</summary>
    /// <remarks>
    /// Response: 204 No Content
    /// </remarks>
    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cancellationToken)
    {
        await service.DeleteRoleAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Removes a permission from a role by permission ID.</summary>
    /// <remarks>
    /// Response: 204 No Content
    /// </remarks>
    [HttpDelete("roles/{id}/permissions/{permissionId}")]
    public async Task<IActionResult> RemoveRolePermission(int id, int permissionId, CancellationToken cancellationToken)
    {
        await service.RemoveRolePermissionAsync(id, permissionId, cancellationToken);
        return NoContent();
    }

    /// <summary>Removes a permission from a role by permission name.</summary>
    /// <remarks>
    /// Response: 204 No Content
    /// </remarks>
    [HttpDelete("roles/{id}/permissions/by-name/{permissionName}")]
    public async Task<IActionResult> RemoveRolePermissionByName(int id, string permissionName, CancellationToken cancellationToken)
    {
        await service.RemoveRolePermissionByNameAsync(id, permissionName, cancellationToken);
        return NoContent();
    }

    /// <summary>Creates a new permission.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "name": "reports:read",
    ///   "description": "Allows reading reports"
    /// }
    /// Response example:
    /// {
    ///   "id": 10,
    ///   "name": "reports:read",
    ///   "description": "Allows reading reports"
    /// }
    /// </remarks>
    [HttpPost("permissions")]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto request, CancellationToken cancellationToken)
        => Ok(await service.CreatePermissionAsync(request, cancellationToken));

    /// <summary>Updates a permission's name and description.</summary>
    /// <remarks>
    /// Request example:
    /// {
    ///   "name": "reports:write",
    ///   "description": "Allows creating and editing reports"
    /// }
    /// Response: 200 OK
    /// </remarks>
    [HttpPut("permissions/{id}")]
    public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionDto request, CancellationToken cancellationToken)
    {
        await service.UpdatePermissionAsync(id, request, cancellationToken);
        return Ok();
    }

    /// <summary>Deletes a permission by its identifier.</summary>
    /// <remarks>
    /// Response: 204 No Content
    /// </remarks>
    [HttpDelete("permissions/{id}")]
    public async Task<IActionResult> DeletePermission(int id, CancellationToken cancellationToken)
    {
        await service.DeletePermissionAsync(id, cancellationToken);
        return NoContent();
    }
}