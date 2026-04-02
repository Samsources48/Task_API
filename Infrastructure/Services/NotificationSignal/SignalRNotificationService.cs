using Api.Hubs;
using Application.Features.Notifications.DTOs;
using Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.NotificationSignal;

public class SignalRNotificationService(IHubContext<NotificationHub, INotificationHub> _hubContext, ILogger<SignalRNotificationService> _logger) : IRealTimeNotifier
{

    public async Task NotifyUserAsync(string clerkId, NotificationDto notification, CancellationToken ct = default)
    {
        _logger.LogInformation("Enviando notificación a usuario {ClerkId}: {Title}", clerkId, notification.Title);
        await _hubContext.Clients.User(clerkId).ReceiveNotification(notification);
    }

    public async Task NotifyAllAsync(NotificationDto notification, CancellationToken ct = default)
    {
        _logger.LogInformation("Enviando notificación global: {Title}", notification.Title);
        await _hubContext.Clients.All.ReceiveNotification(notification);
    }
}
