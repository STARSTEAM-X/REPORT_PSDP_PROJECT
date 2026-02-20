using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Modals;

public class RepairCost
{
    [Key]
    public int CostId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CostName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CostAmount { get; set; }

    [Column(TypeName = "numeric(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal CostUnitPrice { get; set; }

    // 🔥 ไม่ควรให้ client ส่งมา
    [Column(TypeName = "numeric(18,2)")]
    public decimal CostTotal { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int ReportId { get; set; }

    public Report? Report { get; set; }
}