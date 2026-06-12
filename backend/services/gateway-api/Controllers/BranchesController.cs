using HotelOS.GatewayApi.Data;
using HotelOS.GatewayApi.DTOs;
using HotelOS.GatewayApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.GatewayApi.Controllers;

[ApiController]
[Route("api/admin/branches")]
public sealed class BranchesController(GatewayDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var branches = await context.Set<HotelBranch>()
            .OrderBy(b => b.Name)
            .Select(b => new BranchDto(b.Id, b.Name, b.Address, b.City, b.Country, b.Phone, b.Email, b.Status, b.CreatedAt.ToString("yyyy-MM-dd HH:mm")))
            .ToListAsync(cancellationToken);

        return Ok(branches);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var branch = await context.Set<HotelBranch>().FindAsync([id], cancellationToken);
        if (branch is null)
            return NotFound(new { message = "Branch not found" });

        return Ok(new BranchDto(branch.Id, branch.Name, branch.Address, branch.City, branch.Country, branch.Phone, branch.Email, branch.Status, branch.CreatedAt.ToString("yyyy-MM-dd HH:mm")));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchDto request, CancellationToken cancellationToken)
    {
        var branch = new HotelBranch
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            Phone = request.Phone,
            Email = request.Email,
            Status = "Active",
            CreatedAt = DateTimeOffset.UtcNow
        };

        context.Set<HotelBranch>().Add(branch);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = branch.Id }, new BranchDto(branch.Id, branch.Name, branch.Address, branch.City, branch.Country, branch.Phone, branch.Email, branch.Status, branch.CreatedAt.ToString("yyyy-MM-dd HH:mm")));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBranchDto request, CancellationToken cancellationToken)
    {
        var branch = await context.Set<HotelBranch>().FindAsync([id], cancellationToken);
        if (branch is null)
            return NotFound(new { message = "Branch not found" });

        branch.Name = request.Name;
        branch.Address = request.Address;
        branch.City = request.City;
        branch.Country = request.Country;
        branch.Phone = request.Phone;
        branch.Email = request.Email;
        branch.Status = request.Status;

        await context.SaveChangesAsync(cancellationToken);

        return Ok(new BranchDto(branch.Id, branch.Name, branch.Address, branch.City, branch.Country, branch.Phone, branch.Email, branch.Status, branch.CreatedAt.ToString("yyyy-MM-dd HH:mm")));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var branch = await context.Set<HotelBranch>().FindAsync([id], cancellationToken);
        if (branch is null)
            return NotFound(new { message = "Branch not found" });

        context.Set<HotelBranch>().Remove(branch);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
