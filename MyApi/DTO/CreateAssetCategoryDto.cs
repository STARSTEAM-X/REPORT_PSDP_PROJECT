using System.ComponentModel.DataAnnotations;

namespace MyApi.DTO;

public class CreateAssetCategoryDto
{
    [Required]
    public string CategoryName { get; set; } = string.Empty;
}