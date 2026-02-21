namespace MyApi.DTO;

public class ReportResponseDto
{
    public int ReportId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<RepairCostResponseDto>? RepairCosts { get; set; }
}