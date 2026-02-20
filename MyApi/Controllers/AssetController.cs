using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.DTO;
using MyApi.Modals;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AssetController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssetController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var assets = await _context.Assets
            .Include(a => a.Category)
            .ToListAsync();

        return Ok(assets);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAssetDto dto)
    {
        if (await _context.Assets.AnyAsync(a => a.AssetId == dto.AssetId))
            return Conflict("AssetId already exists");

        var asset = new Asset
        {
            AssetId = dto.AssetId.Trim(),
            AssetName = dto.AssetName.Trim(),
            AssetCgId = dto.AssetCgId
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        return Ok(asset);
    }
}