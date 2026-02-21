using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.DTO;
using MyApi.Modals;
using System.Security.Claims;
using System.Text.Json;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // 1️⃣ User สร้าง Report
    // =========================================================
    [HttpPost]
    public async Task<IActionResult> Create(CreateReportDto dto)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var report = new Report
        {
            Title = dto.Title,
            Description = dto.Description,
            LocationId = dto.LocationId,
            AssetId = dto.AssetId,
            ReportOwner = userId,
            Status = ReportStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);

        AddProgressLog(report, "Submitted", "User");

        // แจ้ง Admin
        var admins = await _context.Users
            .Where(u => u.IsAdmin)
            .ToListAsync();

        foreach (var admin in admins)
        {
            _context.Notifications.Add(new Notification
            {
                Title = "มีงานแจ้งซ่อมใหม่",
                Description = report.Title,
                UserId = admin.UserId,
                Report = report
            });
        }

        await _context.SaveChangesAsync();
        return Ok(report);
    }

    // =========================================================
    // 2️⃣ Admin ตรวจสอบ Report
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/review")]
    public async Task<IActionResult> Review(int id, bool isValid)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        if (report.Status != ReportStatus.Submitted &&
            report.Status != ReportStatus.NeedMoreInfo)
            return BadRequest("Invalid status transition");

        if (isValid)
        {
            report.Status = ReportStatus.Accepted;
            AddProgressLog(report, "Accepted", "Admin");
        }
        else
        {
            report.Status = ReportStatus.NeedMoreInfo;
            AddProgressLog(report, "NeedMoreInfo", "Admin");

            _context.Notifications.Add(new Notification
            {
                Title = "กรุณาแก้ไขข้อมูลแจ้งซ่อม",
                Description = report.Title,
                UserId = report.ReportOwner,
                ReportId = report.ReportId
            });
        }

        report.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(report);
    }

    // =========================================================
    // 3️⃣ Admin ส่งงานให้ช่าง (นอกระบบ)
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/start-repair")]
    public async Task<IActionResult> StartRepair(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        if (report.Status != ReportStatus.Accepted)
            return BadRequest("Report must be accepted first");

        report.Status = ReportStatus.InRepair;
        report.UpdatedAt = DateTime.UtcNow;

        AddProgressLog(report, "InRepair", "Admin");

        await _context.SaveChangesAsync();
        return Ok(report);
    }

    // =========================================================
    // 4️⃣ Admin อัปเดตว่างานซ่อมเสร็จแล้ว
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/mark-ready")]
    public async Task<IActionResult> MarkReady(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        if (report.Status != ReportStatus.InRepair)
            return BadRequest("Report must be in repair");

        report.Status = ReportStatus.ReadyToClose;
        report.UpdatedAt = DateTime.UtcNow;

        AddProgressLog(report, "ReadyToClose", "Admin");

        // แจ้ง Owner
        _context.Notifications.Add(new Notification
        {
            Title = "งานซ่อมเสร็จแล้ว",
            Description = report.Title,
            UserId = report.ReportOwner,
            ReportId = report.ReportId
        });

        await _context.SaveChangesAsync();
        return Ok(report);
    }

    // =========================================================
    // 5️⃣ Admin ปิดงาน
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/close")]
    public async Task<IActionResult> Close(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        if (report.Status != ReportStatus.ReadyToClose)
            return BadRequest("Report must be ready to close first");

        report.Status = ReportStatus.Closed;
        report.UpdatedAt = DateTime.UtcNow;

        AddProgressLog(report, "Closed", "Admin");

        await _context.SaveChangesAsync();
        return Ok(report);
    }

    // =========================================================
    // 6️⃣ ดู Report 
    // =========================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReport(int id)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var report = await _context.Reports
            .Include(r => r.RepairCosts)
            .FirstOrDefaultAsync(r => r.ReportId == id);

        if (report == null)
            return NotFound();

        if (!User.IsInRole("Admin") &&
            report.ReportOwner != userId)
            return Forbid();

        return Ok(report);
    }

    // =========================================================
    // 🔹 Helper: Add Progress Log
    // =========================================================
    private void AddProgressLog(Report report, string status, string by)
    {
        var logs = string.IsNullOrEmpty(report.ProgressLog)
            ? new List<ReportProgressEntry>()
            : JsonSerializer.Deserialize<List<ReportProgressEntry>>(report.ProgressLog)
              ?? new List<ReportProgressEntry>();

        logs.Add(new ReportProgressEntry
        {
            Status = status,
            Timestamp = DateTime.UtcNow,
            By = by
        });

        report.ProgressLog = JsonSerializer.Serialize(logs);
    }
}