using HotelOS.RoomService.DTOs;
using HotelOS.RoomService.Services;
using HotelOS.Shared.Storage;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.RoomService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoomController(IRoomQueries queries, IRoomCommands commands, IFileStorage fileStorage) : ControllerBase
{
    [HttpGet("menu")]
    public async Task<IActionResult> GetMenu(CancellationToken cancellationToken)
        => Ok(await queries.GetMenuAsync(cancellationToken));

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
        => Ok(await queries.GetOrdersAsync(cancellationToken));

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateOrderAsync(request, cancellationToken));

    [HttpPut("orders/{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateOrderStatusAsync(id, request, cancellationToken));

    [HttpDelete("orders/{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteOrderAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms(CancellationToken cancellationToken)
        => Ok(await queries.GetRoomsAsync(cancellationToken));

    [HttpGet("rooms/{id:int}")]
    public async Task<IActionResult> GetRoom(int id, CancellationToken cancellationToken)
        => Ok(await queries.GetRoomByIdAsync(id, cancellationToken));

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateRoomAsync(request, cancellationToken));

    [HttpPut("rooms/{id:int}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateRoomAsync(id, request, cancellationToken));

    [HttpPatch("rooms/{id:int}/status")]
    public async Task<IActionResult> PatchRoomStatus(int id, [FromBody] PatchRoomStatusDto request, CancellationToken cancellationToken)
        => Ok(await commands.PatchRoomStatusAsync(id, request, cancellationToken));

    [HttpDelete("rooms/{id:int}")]
    public async Task<IActionResult> DeleteRoom(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteRoomAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
        => Ok(await queries.GetRoomsOverviewAsync(cancellationToken));

    [HttpGet("candidates")]
    public async Task<IActionResult> GetCandidates([FromQuery] int guests = 0, [FromQuery] string? preferredType = null, CancellationToken cancellationToken = default)
        => Ok(await queries.GetAvailableRoomCandidatesAsync(guests, preferredType, cancellationToken));

    [HttpGet("amenities")]
    public async Task<IActionResult> GetAmenities(CancellationToken cancellationToken)
        => Ok(await queries.GetAmenitiesAsync(cancellationToken));

    [HttpPost("upload-image")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext is not ".png" and not ".jpg" and not ".jpeg" and not ".webp")
            return BadRequest("Only PNG, JPG, WebP images are allowed");
        var fileName = $"room_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{ext}";
        await using var stream = file.OpenReadStream();
        await fileStorage.SaveAsync("images/uploads", fileName, stream, cancellationToken);
        return Ok(new UploadImageResponse($"/images/uploads/{fileName}"));
    }
}