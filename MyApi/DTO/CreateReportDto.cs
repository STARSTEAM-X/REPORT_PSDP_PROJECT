using System.ComponentModel.DataAnnotations;

namespace MyApi.DTO;

public class CreateReportDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public string AssetId { get; set; } = string.Empty;
}