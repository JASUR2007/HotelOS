using HotelOS.RoomService.DTOs;
using HotelOS.RoomService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.RoomService.Controllers;

[ApiController]
[Route("api/room/keys")]
public sealed class KeysController(IKeyQueries queries, IKeyCommands commands) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await queries.GetKeysAsync(cancellationToken));

    [HttpGet("room/{roomId:int}")]
    public async Task<IActionResult> GetByRoom(int roomId, CancellationToken cancellationToken)
        => Ok(await queries.GetKeysByRoomAsync(roomId, cancellationToken));

    [HttpPost("issue")]
    public async Task<IActionResult> Issue([FromBody] IssueKeyDto request, CancellationToken cancellationToken)
        => Ok(await commands.IssueKeyAsync(request, cancellationToken));

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> Return(int id, CancellationToken cancellationToken)
        => Ok(await commands.ReturnKeyAsync(id, cancellationToken));

    [HttpPost("{id:int}/lost")]
    public async Task<IActionResult> MarkLost(int id, CancellationToken cancellationToken)
        => Ok(await commands.MarkKeyLostAsync(id, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteKeyAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("master")]
    public async Task<IActionResult> GetMasterKeys(CancellationToken cancellationToken)
        => Ok(await queries.GetMasterKeysAsync(cancellationToken));

    [HttpPost("master")]
    public async Task<IActionResult> CreateMasterKey([FromBody] CreateMasterKeyDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateMasterKeyAsync(request, cancellationToken));

    [HttpPost("master/{id:int}/issue")]
    public async Task<IActionResult> IssueMasterKey(int id, [FromBody] IssueMasterKeyDto request, CancellationToken cancellationToken)
        => Ok(await commands.IssueMasterKeyAsync(id, request, cancellationToken));

    [HttpPost("master/{id:int}/return")]
    public async Task<IActionResult> ReturnMasterKey(int id, CancellationToken cancellationToken)
        => Ok(await commands.ReturnMasterKeyAsync(id, cancellationToken));

    [HttpDelete("master/{id:int}")]
    public async Task<IActionResult> DeleteMasterKey(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteMasterKeyAsync(id, cancellationToken);
        return NoContent();
    }
}
