using Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize]
public class NotificationHub : Hub<INotificationHub>
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        var clerkId = Context.UserIdentifier;
        _logger.LogInformation("Cliente conectado: {ClerkId} | ConnectionId: {ConnectionId}", clerkId, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var clerkId = Context.UserIdentifier;
        _logger.LogInformation("Cliente desconectado: {ClerkId} | ConnectionId: {ConnectionId}", clerkId, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
