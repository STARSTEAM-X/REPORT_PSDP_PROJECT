using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.DTO;
using MyApi.Modals;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LocationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LocationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 ดึงทั้งหมด
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var locations = await _context.Locations
            .OrderBy(l => l.LocationName)
            .ToListAsync();

        return Ok(locations);
    }

    // 🔹 สร้าง Location
    [HttpPost]
    public async Task<IActionResult> Create(CreateLocationDto dto)
    {
        var name = dto.LocationName.Trim();

        if (await _context.Locations.AnyAsync(l => l.LocationName == name))
            return Conflict("Location already exists");

        var location = new Location
        {
            LocationName = name
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return Ok(location);
    }

    // 🔹 ลบ Location (ถ้าไม่มี Report ผูกอยู่)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations
            .Include(l => l.Reports)
            .FirstOrDefaultAsync(l => l.LocationId == id);

        if (location == null)
            return NotFound();

        if (location.Reports != null && location.Reports.Any())
            return BadRequest("Cannot delete location with reports");

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return Ok();
    }
}