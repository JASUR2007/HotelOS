using HotelOS.UserService.DTOs;
using HotelOS.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AdminController(IUserService service) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        => Ok(await service.GetUsersAsync(cancellationToken));

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request, CancellationToken cancellationToken)
        => Ok(await service.CreateUserAsync(request, cancellationToken));

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        await service.DeleteUserAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentDto request, CancellationToken cancellationToken)
    {
        await service.AssignRoleAsync(request, cancellationToken);
        return Ok();
    }

    [HttpPost("assign-permission")]
    public async Task<IActionResult> AssignPermission([FromBody] PermissionAssignmentDto request, CancellationToken cancellationToken)
    {
        await service.AssignPermissionAsync(request, cancellationToken);
        return Ok();
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        => Ok(await service.GetRolesDetailedAsync(cancellationToken));

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
        => Ok(await service.GetPermissionsAsync(cancellationToken));

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto request, CancellationToken cancellationToken)
    {
        await service.UpdateUserAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto request, CancellationToken cancellationToken)
        => Ok(await service.CreateRoleAsync(request, cancellationToken));

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto request, CancellationToken cancellationToken)
    {
        await service.UpdateRoleAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cancellationToken)
    {
        await service.DeleteRoleAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("roles/{id}/permissions/{permissionId}")]
    public async Task<IActionResult> RemoveRolePermission(int id, int permissionId, CancellationToken cancellationToken)
    {
        await service.RemoveRolePermissionAsync(id, permissionId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("roles/{id}/permissions/by-name/{permissionName}")]
    public async Task<IActionResult> RemoveRolePermissionByName(int id, string permissionName, CancellationToken cancellationToken)
    {
        await service.RemoveRolePermissionByNameAsync(id, permissionName, cancellationToken);
        return NoContent();
    }

    [HttpPost("permissions")]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto request, CancellationToken cancellationToken)
        => Ok(await service.CreatePermissionAsync(request, cancellationToken));

    [HttpPut("permissions/{id}")]
    public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionDto request, CancellationToken cancellationToken)
    {
        await service.UpdatePermissionAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpDelete("permissions/{id}")]
    public async Task<IActionResult> DeletePermission(int id, CancellationToken cancellationToken)
    {
        await service.DeletePermissionAsync(id, cancellationToken);
        return NoContent();
    }
}