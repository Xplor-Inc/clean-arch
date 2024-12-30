namespace ShareMarket.Core.Interfaces.Hubs;

public interface INotificationHub
{
    Task SendMessage(string title, string message, CancellationToken cancellationToken = default);
    Task Calculations(string title, string message, CancellationToken cancellationToken = default);
    Task EquityPandit(string title, string message, CancellationToken cancellationToken = default);
    Task Groww(string title, string message, CancellationToken cancellationToken = default);
    Task Screener(string title, string message, CancellationToken cancellationToken = default);
    Task WatchlistSync(string title, string message, CancellationToken cancellationToken = default);
}
