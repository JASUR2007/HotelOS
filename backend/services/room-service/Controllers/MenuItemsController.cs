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
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await context.MenuItems.AsNoTracking().OrderBy(m => m.Name).ToListAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto, CancellationToken ct)
    {
        var item = new MenuItem { Name = dto.Name, Price = dto.Price };
        context.MenuItems.Add(item);
        await context.SaveChangesAsync(ct);
        return Ok(item);
    }

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