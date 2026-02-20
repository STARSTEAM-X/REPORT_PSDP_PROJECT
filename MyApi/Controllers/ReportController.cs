using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.DTO;
using MyApi.Modals;
using System.Security.Claims;

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

    // 🔹 สร้าง Report
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
            Status = ReportStatus.Submitted
        };

        _context.Reports.Add(report);

        // 🔔 บันทึก log ว่ามีการสร้าง report ใหม่
        AddProgressLog(report, "Submitted", "User");
        // 🔔 สร้าง Notification หา Admin
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/assign/{technicianId}")]
    public async Task<IActionResult> Assign(int id, int technicianId)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        report.ReportTechnician = technicianId;
        report.Status = ReportStatus.Inspecting;
        report.UpdatedAt = DateTime.UtcNow;

        AddProgressLog(report, "Inspecting", "Admin");
        // แจ้ง Technician
        _context.Notifications.Add(new Notification
        {
            Title = "คุณได้รับงานใหม่",
            Description = report.Title,
            UserId = technicianId,
            ReportId = report.ReportId
        });

        await _context.SaveChangesAsync();

        return Ok(report);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, ReportStatus status)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        report.Status = status;
        report.UpdatedAt = DateTime.UtcNow;

        // แจ้ง Owner
        _context.Notifications.Add(new Notification
        {
            Title = "สถานะงานเปลี่ยน",
            Description = $"งาน {report.Title} เปลี่ยนเป็น {status}",
            UserId = report.ReportOwner,
            ReportId = report.ReportId
        });

        await _context.SaveChangesAsync();

        return Ok(report);
    }

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
            report.ReportOwner != userId &&
            report.ReportTechnician != userId)
            return Forbid();

        return Ok(report);
    }

    [Authorize(Roles = "Technician")]
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteReport(int id)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var report = await _context.Reports.FindAsync(id);
        if (report == null)
            return NotFound();

        // 🔒 ต้องเป็นงานที่ assign ให้ technician คนนี้
        if (report.ReportTechnician != userId)
            return Forbid();

        // 🔒 ต้องอยู่ในสถานะ InRepair เท่านั้น
        if (report.Status != ReportStatus.InRepair)
            return BadRequest("Report is not in repair status");

        report.Status = ReportStatus.ReadyToClose;
        report.UpdatedAt = DateTime.UtcNow;

        // 🔔 บันทึก log ว่าช่างทำงานเสร็จแล้ว
        AddProgressLog(report, "ReadyToClose", "Technician");
        // 🔔 แจ้ง Owner
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/close")]
    public async Task<IActionResult> CloseReport(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null)
            return NotFound();

        if (report.Status != ReportStatus.ReadyToClose)
            return BadRequest("Report must be ready to close first");

        AddProgressLog(report, "Closed", "Admin");

        report.Status = ReportStatus.Closed;
        report.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(report);
    }



    private void AddProgressLog(Report report, string status, string by)
    {
        var logs = string.IsNullOrEmpty(report.ProgressLog)
            ? new List<ReportProgressEntry>()
            : System.Text.Json.JsonSerializer
                .Deserialize<List<ReportProgressEntry>>(report.ProgressLog)
                ?? new List<ReportProgressEntry>();

        logs.Add(new ReportProgressEntry
        {
            Status = status,
            Timestamp = DateTime.UtcNow,
            By = by
        });

        report.ProgressLog = System.Text.Json.JsonSerializer.Serialize(logs);
    }
}