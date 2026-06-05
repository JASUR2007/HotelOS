using HotelOS.MaintenanceService.DTOs;
using HotelOS.MaintenanceService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.MaintenanceService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MaintenanceController(
    IMaintenanceQueries queries,
    IMaintenanceCommands commands) : ControllerBase
{
    /// <summary>Retrieves all maintenance issues.</summary>
    /// <remarks>
    /// Example response:
    /// <code>
    /// [
    ///   {
    ///     "id": 1,
    ///     "roomNumber": "101",
    ///     "title": "Air conditioning not working",
    ///     "priority": "High",
    ///     "status": "InProgress",
    ///     "technicianName": "Bob Johnson"
    ///   }
    /// ]
    /// </code>
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await queries.GetIssuesAsync(cancellationToken));

    /// <summary>Creates a new maintenance issue.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// {
    ///   "roomNumber": "101",
    ///   "title": "Leaking faucet",
    ///   "priority": "Medium"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIssueDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateIssueAsync(request, cancellationToken));

    /// <summary>Assigns a technician to an existing maintenance issue.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// {
    ///   "issueId": 1,
    ///   "technicianName": "Bob Johnson"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignTechnicianDto request, CancellationToken cancellationToken)
        => Ok(await commands.AssignTechnicianAsync(request, cancellationToken));

    /// <summary>Updates an existing maintenance issue by ID.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// {
    ///   "status": "Completed",
    ///   "technicianName": "Bob Johnson",
    ///   "priority": "Low"
    /// }
    /// </code>
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIssueDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateIssueAsync(id, request, cancellationToken));

    /// <summary>Deletes a maintenance issue by ID.</summary>
    /// <remarks>
    /// Example request: DELETE /api/maintenance/1
    /// Response: 204 No Content
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteIssueAsync(id, cancellationToken);
        return NoContent();
    }
}
