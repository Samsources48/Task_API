using Application.Features.Notifications.DTOs;

namespace Application.Features.Notifications.Interfaces;

public interface IRealTimeNotifier
{
    Task NotifyUserAsync(string clerkId, NotificationDto notification, CancellationToken ct = default);
    Task NotifyAllAsync(NotificationDto notification, CancellationToken ct = default);
}
