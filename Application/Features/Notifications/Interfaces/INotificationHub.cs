namespace Application.Features.Notifications.Interfaces;

public interface INotificationHub
{
    Task ReceiveNotification(object notification);
}
