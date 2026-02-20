using System.ComponentModel.DataAnnotations;

namespace MyApi.DTO;

public class CreateLocationDto
{
    [Required]
    public string LocationName { get; set; } = string.Empty;
}