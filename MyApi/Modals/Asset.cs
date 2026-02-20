using System.ComponentModel.DataAnnotations;

namespace MyApi.Modals;

public class Asset
{
    [Key]
    [MaxLength(50)]
    public string AssetId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AssetName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int AssetCgId { get; set; }

    public AssetCategory? Category { get; set; }
}