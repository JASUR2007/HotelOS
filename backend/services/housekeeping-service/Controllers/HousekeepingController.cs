using HotelOS.HousekeepingService.DTOs;
using HotelOS.HousekeepingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.HousekeepingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HousekeepingController(IHousekeepingService service) : ControllerBase
{
    /// <summary>Retrieves the entire housekeeping task queue.</summary>
    /// <remarks>
    /// GET /api/housekeeping/queue
    ///
    /// Response body example:
    /// [
    ///   {
    ///     "roomId": 1,
    ///     "roomNumber": "101",
    ///     "status": "Pending",
    ///     "assignedTo": "Alice Smith"
    ///   },
    ///   {
    ///     "roomId": 2,
    ///     "roomNumber": "102",
    ///     "status": "InProgress",
    ///     "assignedTo": "Bob Johnson"
    ///   }
    /// ]
    /// </remarks>
    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue(CancellationToken cancellationToken)
        => Ok(await service.GetQueueAsync(cancellationToken));

    /// <summary>Updates the status of an existing cleaning task.</summary>
    /// <remarks>
    /// PUT /api/housekeeping/status
    ///
    /// Request body example:
    /// {
    ///   "taskId": 1,
    ///   "status": "Completed",
    ///   "assignedTo": "Alice Smith"
    /// }
    ///
    /// Response body example:
    /// {
    ///   "roomId": 1,
    ///   "roomNumber": "101",
    ///   "status": "Completed",
    ///   "assignedTo": "Alice Smith"
    /// }
    /// </remarks>
    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateCleaningStatusDto request, CancellationToken cancellationToken)
        => Ok(await service.UpdateStatusAsync(request, cancellationToken));

    /// <summary>Creates a new cleaning task in the queue.</summary>
    /// <remarks>
    /// POST /api/housekeeping/queue
    ///
    /// Request body example:
    /// {
    ///   "roomNumber": "201",
    ///   "assignedTo": "Charlie Brown",
    ///   "priority": "High"
    /// }
    ///
    /// Response body example:
    /// {
    ///   "roomId": 3,
    ///   "roomNumber": "201",
    ///   "status": "Pending",
    ///   "assignedTo": "Charlie Brown"
    /// }
    /// </remarks>
    [HttpPost("queue")]
    public async Task<IActionResult> CreateTask([FromBody] CreateCleaningTaskDto request, CancellationToken cancellationToken)
        => Ok(await service.CreateTaskAsync(request, cancellationToken));

    /// <summary>Updates an existing cleaning task by its identifier.</summary>
    /// <remarks>
    /// PUT /api/housekeeping/queue/{id}
    ///
    /// Request body example:
    /// {
    ///   "status": "InProgress",
    ///   "assignedTo": "Diana Prince"
    /// }
    ///
    /// Response body example:
    /// {
    ///   "roomId": 1,
    ///   "roomNumber": "101",
    ///   "status": "InProgress",
    ///   "assignedTo": "Diana Prince"
    /// }
    /// </remarks>
    [HttpPut("queue/{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateCleaningTaskDto request, CancellationToken cancellationToken)
        => Ok(await service.UpdateTaskAsync(id, request, cancellationToken));

    /// <summary>Deletes a cleaning task from the queue.</summary>
    /// <remarks>
    /// DELETE /api/housekeeping/queue/{id}
    ///
    /// Path parameter: id (integer) — The task identifier. Example: 1.
    ///
    /// Response: 204 No Content
    /// </remarks>
    [HttpDelete("queue/{id}")]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
    {
        await service.DeleteTaskAsync(id, cancellationToken);
        return NoContent();
    }
}