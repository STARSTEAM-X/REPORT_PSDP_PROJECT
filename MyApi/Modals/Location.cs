using System.ComponentModel.DataAnnotations;

namespace MyApi.Modals;

public class Location
{
    [Key]
    public int LocationId { get; set; }

    [Required]
    [MaxLength(150)]
    public string LocationName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Report>? Reports { get; set; }
}