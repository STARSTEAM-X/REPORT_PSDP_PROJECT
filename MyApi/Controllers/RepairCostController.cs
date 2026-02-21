using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.DTO;
using MyApi.Modals;

[ApiController]
[Route("api/reports/{reportId}/costs")]
[Authorize]
public class RepairCostController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RepairCostController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // 🔹 เพิ่มค่าใช้จ่าย
    // POST /api/reports/{reportId}/costs
    // =========================================================
    [HttpPost]
    public async Task<IActionResult> AddCost(int reportId, [FromBody] CreateRepairCostDto dto)
    {
        var reportExists = await _context.Reports
            .AnyAsync(r => r.ReportId == reportId);

        if (!reportExists)
            return NotFound(new { message = "Report not found" });

        var cost = new RepairCost
        {
            ReportId = reportId,
            CostName = dto.CostName.Trim(),
            CostAmount = dto.CostAmount,
            CostUnitPrice = dto.CostUnitPrice,
            CostTotal = dto.CostAmount * dto.CostUnitPrice
        };

        _context.RepairCosts.Add(cost);
        await _context.SaveChangesAsync();

        var response = new RepairCostResponseDto
        {
            CostId = cost.CostId,
            CostName = cost.CostName,
            CostAmount = cost.CostAmount,
            CostUnitPrice = cost.CostUnitPrice,
            CostTotal = cost.CostTotal,
            CreatedAt = cost.CreatedAt,
            ReportId = cost.ReportId
        };

        return Ok(response);
    }

    // =========================================================
    // 🔹 ดูรายการค่าใช้จ่ายทั้งหมด
    // GET /api/reports/{reportId}/costs
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> GetAllCosts(int reportId)
    {
        var costs = await _context.RepairCosts
            .Where(c => c.ReportId == reportId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new RepairCostResponseDto
            {
                CostId = c.CostId,
                CostName = c.CostName,
                CostAmount = c.CostAmount,
                CostUnitPrice = c.CostUnitPrice,
                CostTotal = c.CostTotal,
                CreatedAt = c.CreatedAt,
                ReportId = c.ReportId
            })
            .ToListAsync();

        return Ok(costs);
    }

    // =========================================================
    // 🔹 ดูค่าใช้จ่ายรายการเดียว
    // GET /api/reports/{reportId}/costs/{costId}
    // =========================================================
    [HttpGet("{costId}")]
    public async Task<IActionResult> GetCost(int reportId, int costId)
    {
        var cost = await _context.RepairCosts
            .Where(c => c.ReportId == reportId && c.CostId == costId)
            .Select(c => new RepairCostResponseDto
            {
                CostId = c.CostId,
                CostName = c.CostName,
                CostAmount = c.CostAmount,
                CostUnitPrice = c.CostUnitPrice,
                CostTotal = c.CostTotal,
                CreatedAt = c.CreatedAt,
                ReportId = c.ReportId
            })
            .FirstOrDefaultAsync();

        if (cost == null)
            return NotFound(new { message = "Cost not found" });

        return Ok(cost);
    }

    // =========================================================
    // 🔹 รวมยอดทั้งหมด
    // GET /api/reports/{reportId}/costs/total
    // =========================================================
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalCost(int reportId)
    {
        var total = await _context.RepairCosts
            .Where(c => c.ReportId == reportId)
            .SumAsync(c => (decimal?)c.CostTotal) ?? 0;

        return Ok(new { total });
    }

    // =========================================================
    // 🔹 ลบค่าใช้จ่าย
    // DELETE /api/reports/{reportId}/costs/{costId}
    // =========================================================
    [HttpDelete("{costId}")]
    public async Task<IActionResult> DeleteCost(int reportId, int costId)
    {
        var cost = await _context.RepairCosts
            .FirstOrDefaultAsync(c =>
                c.ReportId == reportId &&
                c.CostId == costId);

        if (cost == null)
            return NotFound(new { message = "Cost not found" });

        _context.RepairCosts.Remove(cost);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Deleted successfully" });
    }
}