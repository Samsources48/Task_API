
namespace Infrastructure.Services.Notification.Email
{
    public interface IEmailNotificationService
    {
        //Task NotifyTrackingCompletedAsync( List<TrackingResultDto> resultados, DateTime fechaInicio, CancellationToken ct = default);
        Task NotifyExpiringTaskAsync(string recipientEmail, string userName, string taskTitle, DateTime? dueDate, CancellationToken ct = default);
    }
}
