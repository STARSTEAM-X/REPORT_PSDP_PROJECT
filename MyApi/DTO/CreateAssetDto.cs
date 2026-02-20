using System.ComponentModel.DataAnnotations;

namespace MyApi.DTO;

public class CreateAssetDto
{
    [Required]
    public string AssetId { get; set; } = string.Empty;

    [Required]
    public string AssetName { get; set; } = string.Empty;

    [Required]
    public int AssetCgId { get; set; }
}