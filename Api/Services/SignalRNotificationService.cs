using Api.Hubs;
using Application.Features.Notifications.DTOs;
using Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services;

public class SignalRNotificationService : IRealTimeNotifier
{
    private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(IHubContext<NotificationHub, INotificationHub> hubContext, ILogger<SignalRNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

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
