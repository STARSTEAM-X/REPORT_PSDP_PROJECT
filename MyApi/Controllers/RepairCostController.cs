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

    // 🔹 เพิ่มค่าใช้จ่าย POST /api/reports/5/costs
    [HttpPost]
    public async Task<IActionResult> AddCost(int reportId, CreateRepairCostDto dto)
    {
        var report = await _context.Reports.FindAsync(reportId);
        if (report == null)
            return NotFound("Report not found");

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

        return Ok(cost);
    }

    // 🔹 ดูรายการค่าใช้จ่ายทั้งหมดของ report GET /api/reports/5/costs
    [HttpGet]
    public async Task<IActionResult> GetAllCosts(int reportId)
    {
        var costs = await _context.RepairCosts
            .Where(c => c.ReportId == reportId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(costs);
    }

    // 🔹 ดูรายละเอียดค่าใช้จ่ายรายการเดียว GET /api/reports/5/costs
    [HttpGet("{costId}")]
    public async Task<IActionResult> GetCost(int reportId, int costId)
    {
        var cost = await _context.RepairCosts
            .FirstOrDefaultAsync(c =>
                c.ReportId == reportId &&
                c.CostId == costId);

        if (cost == null)
            return NotFound();

        return Ok(cost);
    }

    // 🔹 รวมยอดทั้งหมดของ report GET /api/reports/5/costs/total
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalCost(int reportId)
    {
        var total = await _context.RepairCosts
            .Where(c => c.ReportId == reportId)
            .SumAsync(c => c.CostTotal);

        return Ok(new { total });
    }

    // 🔹 ลบค่าใช้จ่าย DELETE /api/reports/5/costs/3
    [HttpDelete("{costId}")] 
    public async Task<IActionResult> DeleteCost(int reportId, int costId)
    {
        var cost = await _context.RepairCosts
            .FirstOrDefaultAsync(c =>
                c.ReportId == reportId &&
                c.CostId == costId);

        if (cost == null)
            return NotFound();

        _context.RepairCosts.Remove(cost);
        await _context.SaveChangesAsync();

        return Ok("Deleted");
    }
}