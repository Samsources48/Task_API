
namespace Infrastructure.Services.Notification.Email
{
    public interface IEmailNotificationService
    {
        Task NotifyExpiringTaskAsync(string recipientEmail, string userName, string taskTitle, DateTime? dueDate, CancellationToken ct = default);
    }
}
