using Microsoft.AspNetCore.SignalR;
using ShareMarket.Core.Interfaces.Hubs;

namespace ShareMarket.Core.Hubs;

public class NotificationHub(IHubContext<NotificationHub> HubContext) : Hub, INotificationHub
{
    public async Task SendMessage(string title, string message, CancellationToken cancellationToken = default)
    {
        await HubContext.Clients.All.SendAsync("SendMessage", title, message, cancellationToken: cancellationToken);
    }

    public async Task Calculations(string title, string message, CancellationToken cancellationToken = default)
    {
        await HubContext.Clients.All.SendAsync("Calculations", title, message, cancellationToken: cancellationToken);
    }
    public async Task EquityPandit(string title, string message, CancellationToken cancellationToken = default)
    {
        await HubContext.Clients.All.SendAsync("EquityPandit", title, message, cancellationToken: cancellationToken);
    }
    public async Task Groww(string title, string message, CancellationToken cancellationToken = default)
    {
        await HubContext.Clients.All.SendAsync("Groww", title, message, cancellationToken: cancellationToken);
    }
    public async Task Screener(string title, string message, CancellationToken cancellationToken = default)
    {
        await HubContext.Clients.All.SendAsync("Screener", title, message, cancellationToken: cancellationToken);
    }
    public async Task WatchlistSync(string title, string message, CancellationToken cancellationToken = default)
    {
        await HubContext.Clients.All.SendAsync("WatchlistSync", title, message, cancellationToken: cancellationToken);
    }
}