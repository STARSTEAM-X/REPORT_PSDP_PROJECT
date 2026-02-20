using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Modals;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public string UserEmail { get; set; } = string.Empty;

    [Required]
    public string UserPassword { get; set; } = string.Empty;

    public string? UserFullname { get; set; }

    public string? UserPhone { get; set; }

    public bool IsAdmin { get; set; } = false;

    // Navigation
    public ICollection<Report>? Reports { get; set; }
}