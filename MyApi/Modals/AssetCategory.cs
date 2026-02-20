using System.ComponentModel.DataAnnotations;

namespace MyApi.Modals;

public class AssetCategory
{
    [Key]
    public int AssetCgId { get; set; }

    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public ICollection<Asset>? Assets { get; set; }
}