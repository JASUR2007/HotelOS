using HotelOS.RoomService.DTOs;
using HotelOS.RoomService.Services;
using HotelOS.Shared.Storage;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.RoomService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoomController(IRoomQueries queries, IRoomCommands commands, IFileStorage fileStorage) : ControllerBase
{
    /// <summary>
    /// Retrieves the full room-service menu.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/menu
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       {
    ///         "id": 1,
    ///         "name": "Club Sandwich",
    ///         "price": 12.99
    ///       }
    ///     ]
    /// </remarks>
    [HttpGet("menu")]
    public async Task<IActionResult> GetMenu(CancellationToken cancellationToken)
        => Ok(await queries.GetMenuAsync(cancellationToken));

    /// <summary>
    /// Retrieves all room-service orders.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/orders
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       {
    ///         "id": 1,
    ///         "roomNumber": "101",
    ///         "guestName": "John Doe",
    ///         "status": "Pending",
    ///         "total": 25.98
    ///       }
    ///     ]
    /// </remarks>
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
        => Ok(await queries.GetOrdersAsync(cancellationToken));

    /// <summary>
    /// Creates a new room-service order.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/room/orders
    ///     {
    ///       "roomNumber": "101",
    ///       "guestName": "John Doe",
    ///       "items": ["Club Sandwich", "French Fries"]
    ///     }
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 1,
    ///       "roomNumber": "101",
    ///       "guestName": "John Doe",
    ///       "status": "Pending",
    ///       "total": 25.98
    ///     }
    /// </remarks>
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateOrderAsync(request, cancellationToken));

    /// <summary>
    /// Updates the status of an existing order.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/room/orders/1/status
    ///     {
    ///       "status": "Delivered"
    ///     }
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 1,
    ///       "roomNumber": "101",
    ///       "guestName": "John Doe",
    ///       "status": "Delivered",
    ///       "total": 25.98
    ///     }
    /// </remarks>
    [HttpPut("orders/{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateOrderStatusAsync(id, request, cancellationToken));

    /// <summary>
    /// Deletes an order by its ID.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/room/orders/1
    /// 
    /// Sample response (204 No Content)
    /// </remarks>
    [HttpDelete("orders/{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteOrderAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Retrieves all rooms.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/rooms
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       {
    ///         "id": 1,
    ///         "roomNumber": "101",
    ///         "type": "Deluxe",
    ///         "status": "Available",
    ///         "pricePerNight": 150.00,
    ///         "floor": 2,
    ///         "description": "Spacious room with city view",
    ///         "guestCapacity": 2,
    ///         "mainImage": "/images/rooms/room-1.png",
    ///         "images": ["/images/rooms/room-1a.png"],
    ///         "amenities": ["WiFi", "Air Conditioning", "TV"]
    ///       }
    ///     ]
    /// </remarks>
    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms(CancellationToken cancellationToken)
        => Ok(await queries.GetRoomsAsync(cancellationToken));

    /// <summary>
    /// Retrieves a single room by its ID.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/rooms/1
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 1,
    ///       "roomNumber": "101",
    ///       "type": "Deluxe",
    ///       "status": "Available",
    ///       "pricePerNight": 150.00,
    ///       "floor": 2,
    ///       "description": "Spacious room with city view",
    ///       "guestCapacity": 2,
    ///       "mainImage": "/images/rooms/room-1.png",
    ///       "images": ["/images/rooms/room-1a.png"],
    ///       "amenities": ["WiFi", "Air Conditioning", "TV"]
    ///     }
    /// </remarks>
    [HttpGet("rooms/{id:int}")]
    public async Task<IActionResult> GetRoom(int id, CancellationToken cancellationToken)
        => Ok(await queries.GetRoomByIdAsync(id, cancellationToken));

    /// <summary>
    /// Creates a new room.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/room/rooms
    ///     {
    ///       "roomNumber": "103",
    ///       "type": "Deluxe",
    ///       "floor": 1,
    ///       "pricePerNight": 150.00,
    ///       "guestCapacity": 2,
    ///       "description": "Spacious room with city view",
    ///       "mainImage": "/images/rooms/room-3.png",
    ///       "images": ["/images/rooms/room-3a.png"],
    ///       "amenityIds": ["1", "2", "3"]
    ///     }
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 3,
    ///       "roomNumber": "103",
    ///       "type": "Deluxe",
    ///       "status": "Available",
    ///       "pricePerNight": 150.00,
    ///       "floor": 1,
    ///       "description": "Spacious room with city view",
    ///       "guestCapacity": 2,
    ///       "mainImage": "/images/rooms/room-3.png",
    ///       "images": ["/images/rooms/room-3a.png"],
    ///       "amenities": ["WiFi", "Air Conditioning", "TV"]
    ///     }
    /// </remarks>
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateRoomAsync(request, cancellationToken));

    /// <summary>
    /// Updates an existing room.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/room/rooms/1
    ///     {
    ///       "roomNumber": "101",
    ///       "type": "Deluxe",
    ///       "status": "Occupied",
    ///       "pricePerNight": 180.00,
    ///       "floor": 2,
    ///       "description": "Renovated deluxe room with city view",
    ///       "guestCapacity": 3,
    ///       "mainImage": "/images/rooms/room-1.png",
    ///       "images": ["/images/rooms/room-1a.png"],
    ///       "amenities": ["WiFi", "Air Conditioning", "TV", "Mini Bar"]
    ///     }
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 1,
    ///       "roomNumber": "101",
    ///       "type": "Deluxe",
    ///       "status": "Occupied",
    ///       "pricePerNight": 180.00,
    ///       "floor": 2,
    ///       "description": "Renovated deluxe room with city view",
    ///       "guestCapacity": 3,
    ///       "mainImage": "/images/rooms/room-1.png",
    ///       "images": ["/images/rooms/room-1a.png"],
    ///       "amenities": ["WiFi", "Air Conditioning", "TV", "Mini Bar"]
    ///     }
    /// </remarks>
    [HttpPut("rooms/{id:int}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDto request, CancellationToken cancellationToken)
        => Ok(await commands.UpdateRoomAsync(id, request, cancellationToken));

    /// <summary>
    /// Partially updates the status of a room.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PATCH /api/room/rooms/1/status
    ///     {
    ///       "status": "Maintenance"
    ///     }
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 1,
    ///       "roomNumber": "101",
    ///       "type": "Deluxe",
    ///       "status": "Maintenance",
    ///       "pricePerNight": 150.00,
    ///       "floor": 2,
    ///       "description": "Spacious room with city view",
    ///       "guestCapacity": 2,
    ///       "mainImage": "/images/rooms/room-1.png",
    ///       "images": ["/images/rooms/room-1a.png"],
    ///       "amenities": ["WiFi", "Air Conditioning", "TV"]
    ///     }
    /// </remarks>
    [HttpPatch("rooms/{id:int}/status")]
    public async Task<IActionResult> PatchRoomStatus(int id, [FromBody] PatchRoomStatusDto request, CancellationToken cancellationToken)
        => Ok(await commands.PatchRoomStatusAsync(id, request, cancellationToken));

    /// <summary>
    /// Deletes a room by its ID.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/room/rooms/1
    /// 
    /// Sample response (204 No Content)
    /// </remarks>
    [HttpDelete("rooms/{id:int}")]
    public async Task<IActionResult> DeleteRoom(int id, CancellationToken cancellationToken)
    {
        await commands.DeleteRoomAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Retrieves a dashboard overview of all rooms.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/overview
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       {
    ///         "id": 1,
    ///         "roomNumber": "101",
    ///         "status": "Available",
    ///         "guestName": null,
    ///         "housekeeping": "Cleaning"
    ///       }
    ///     ]
    /// </remarks>
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
        => Ok(await queries.GetRoomsOverviewAsync(cancellationToken));

    /// <summary>
    /// Retrieves available rooms that can be assigned as candidates.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/candidates
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       {
    ///         "id": 1,
    ///         "roomNumber": "101",
    ///         "type": "Deluxe",
    ///         "status": "Available",
    ///         "pricePerNight": 150.00,
    ///         "floor": 2,
    ///         "description": "Spacious room with city view",
    ///         "guestCapacity": 2,
    ///         "mainImage": "/images/rooms/room-1.png",
    ///         "images": ["/images/rooms/room-1a.png"],
    ///         "amenities": ["WiFi", "Air Conditioning", "TV"]
    ///       }
    ///     ]
    /// </remarks>
    [HttpGet("candidates")]
    public async Task<IActionResult> GetCandidates(CancellationToken cancellationToken)
        => Ok(await queries.GetAvailableRoomCandidatesAsync(cancellationToken));

    /// <summary>
    /// Retrieves all available amenities.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/amenities
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       { "id": 1, "name": "WiFi", "iconUrl": "/images/amenities/wifi.svg", "description": "High-speed internet access" },
    ///       { "id": 2, "name": "Coffee Machine", "iconUrl": "/images/amenities/coffee.svg", "description": "Coffee machine with complimentary pods" }
    ///     ]
    /// </remarks>
    [HttpGet("amenities")]
    public async Task<IActionResult> GetAmenities(CancellationToken cancellationToken)
        => Ok(await queries.GetAmenitiesAsync(cancellationToken));

    /// <summary>
    /// Uploads a room image and returns its public URL.
    /// </summary>
    /// <remarks>
    /// Sample request (multipart/form-data):
    /// 
    ///     POST /api/room/upload-image
    ///     Content-Type: multipart/form-data
    ///     file: [binary image data]
    /// 
    /// Sample response (200 OK):
    /// 
    ///     { "url": "/images/rooms/room_20260604_120000.png" }
    /// </remarks>
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
