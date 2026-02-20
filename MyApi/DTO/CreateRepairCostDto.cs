using System.ComponentModel.DataAnnotations;

namespace MyApi.DTO;

public class CreateRepairCostDto
{
    [Required]
    public string CostName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CostAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CostUnitPrice { get; set; }
}