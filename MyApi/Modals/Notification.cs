using System.ComponentModel.DataAnnotations;

namespace MyApi.Modals;

public class Notification
{
    [Key]
    public int NotifiId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsRead { get; set; } = false;

    public int UserId { get; set; }
    public int? ReportId { get; set; }

    public User? User { get; set; }
    public Report? Report { get; set; }
}