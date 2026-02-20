namespace MyApi.DTO;

public class CreateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserId { get; set; }
    public int? ReportId { get; set; }
}