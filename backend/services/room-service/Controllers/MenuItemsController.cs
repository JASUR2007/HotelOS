using HotelOS.RoomService.Data;
using HotelOS.RoomService.DTOs;
using HotelOS.RoomService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.RoomService.Controllers;

[ApiController]
[Route("api/room/[controller]")]
public sealed class MenuItemsController(RoomDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all menu items sorted by name.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/room/menuitems
    /// 
    /// Sample response (200 OK):
    /// 
    ///     [
    ///       {
    ///         "id": 1,
    ///         "name": "Club Sandwich",
    ///         "price": 12.99
    ///       },
    ///       {
    ///         "id": 2,
    ///         "name": "French Fries",
    ///         "price": 5.99
    ///       }
    ///     ]
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await context.MenuItems.AsNoTracking().OrderBy(m => m.Name).ToListAsync(ct));

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/room/menuitems
    ///     {
    ///       "name": "Club Sandwich",
    ///       "price": 12.99
    ///     }
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 3,
    ///       "name": "Club Sandwich",
    ///       "price": 12.99
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto, CancellationToken ct)
    {
        var item = new MenuItem { Name = dto.Name, Price = dto.Price };
        context.MenuItems.Add(item);
        await context.SaveChangesAsync(ct);
        return Ok(item);
    }

    /// <summary>
    /// Updates an existing menu item.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/room/menuitems/1
    ///     {
    ///       "name": "Club Sandwich Deluxe",
    ///       "price": 14.99
    ///     }
    /// 
    /// Sample response (200 OK):
    /// 
    ///     {
    ///       "id": 1,
    ///       "name": "Club Sandwich Deluxe",
    ///       "price": 14.99
    ///     }
    /// 
    /// Sample response (404 Not Found) when the item does not exist.
    /// </remarks>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto dto, CancellationToken ct)
    {
        var item = await context.MenuItems.FindAsync([id], ct);
        if (item is null) return NotFound();
        item.Name = dto.Name;
        item.Price = dto.Price;
        await context.SaveChangesAsync(ct);
        return Ok(item);
    }

    /// <summary>
    /// Deletes a menu item by its ID.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/room/menuitems/1
    /// 
    /// Sample response (204 No Content)
    /// 
    /// Sample response (404 Not Found) when the item does not exist.
    /// </remarks>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await context.MenuItems.FindAsync([id], ct);
        if (item is null) return NotFound();
        context.MenuItems.Remove(item);
        await context.SaveChangesAsync(ct);
        return NoContent();
    }
}
