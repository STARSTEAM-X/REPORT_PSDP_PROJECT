using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.DTO;
using MyApi.Modals;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AssetCategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssetCategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.AssetCategories.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAssetCategoryDto dto)
    {
        var name = dto.CategoryName.Trim();

        if (await _context.AssetCategories.AnyAsync(c => c.CategoryName == name))
            return Conflict("Category already exists");

        var category = new AssetCategory
        {
            CategoryName = name
        };

        _context.AssetCategories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(category);
    }
}