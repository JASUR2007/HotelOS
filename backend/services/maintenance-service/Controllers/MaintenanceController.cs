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
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await queries.GetIssuesAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIssueDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateIssueAsync(request, cancellationToken));

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignTechnicianDto request, CancellationToken cancellationToken)
        => Ok(await commands.AssignTechnicianAsync(request, cancellationToken));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIssueDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateIssueAsync(id, request, cancellationToken));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteIssueAsync(id, cancellationToken);
        return NoContent();
    }
}
