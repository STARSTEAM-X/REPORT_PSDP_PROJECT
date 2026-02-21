using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Modals;

public class Report
{
    [Key]
    public int ReportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageBefore { get; set; }

    public string? ImageAfter { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Submitted;

    public string? ProgressLog { get; set; } // JSON string

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int ReportOwner { get; set; }
    public int? ReportTechnician { get; set; }
    public int LocationId { get; set; }
    public string AssetId { get; set; } = string.Empty;

    // Navigation
    public User? Owner { get; set; }
    public User? Technician { get; set; }
    public Location? Location { get; set; }
    public Asset? Asset { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<RepairCost>? RepairCosts { get; set; }
}