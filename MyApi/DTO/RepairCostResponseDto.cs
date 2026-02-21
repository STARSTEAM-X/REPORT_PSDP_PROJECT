namespace MyApi.DTO;

public class RepairCostResponseDto
{
    public int CostId { get; set; }
    public string CostName { get; set; } = string.Empty;
    public int CostAmount { get; set; }
    public decimal CostUnitPrice { get; set; }
    public decimal CostTotal { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ReportId { get; set; }
}