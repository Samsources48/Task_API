namespace Application.Features.Notifications.DTOs;

public class NotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long? TaskId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
