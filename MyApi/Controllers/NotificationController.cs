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
public class NotificationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 ดึง notification ของ user ปัจจุบัน
    // api/notification
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.NotifiId)
            .ToListAsync();

        return Ok(notifications);
    }

    // 🔹 นับ unread
    // api/notification/unread-count
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var count = await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);

        return Ok(new { unread = count });
    }

    // 🔹 mark as read
    // api/notification/{id}/read
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);

        if (notification == null)
            return NotFound();

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok();
    }

    // 🔹 สร้าง notification (ใช้ภายในระบบ)
    [HttpPost]
    public async Task<IActionResult> Create(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            Title = dto.Title,
            Description = dto.Description,
            UserId = dto.UserId,
            ReportId = dto.ReportId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return Ok(notification);
    }
}