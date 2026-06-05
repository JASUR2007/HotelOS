using HotelOS.HousekeepingService.DTOs;
using HotelOS.HousekeepingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.HousekeepingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HousekeepingController(IHousekeepingService service) : ControllerBase
{
    /// <summary>Retrieves the entire housekeeping task queue.</summary>
    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue(CancellationToken cancellationToken)
        => Ok(await service.GetQueueAsync(cancellationToken));

    /// <summary>Updates the status of an existing cleaning task.</summary>
    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateCleaningStatusDto request, CancellationToken cancellationToken)
        => Ok(await service.UpdateStatusAsync(request, cancellationToken));

    /// <summary>Creates a new cleaning task in the queue.</summary>
    [HttpPost("queue")]
    public async Task<IActionResult> CreateTask([FromBody] CreateCleaningTaskDto request, CancellationToken cancellationToken)
        => Ok(await service.CreateTaskAsync(request, cancellationToken));

    /// <summary>Updates an existing cleaning task by ID.</summary>
    [HttpPut("queue/{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateCleaningTaskDto request, CancellationToken cancellationToken)
        => Ok(await service.UpdateTaskAsync(id, request, cancellationToken));

    /// <summary>Deletes a cleaning task from the queue.</summary>
    [HttpDelete("queue/{id}")]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
    {
        await service.DeleteTaskAsync(id, cancellationToken);
        return NoContent();
    }
}