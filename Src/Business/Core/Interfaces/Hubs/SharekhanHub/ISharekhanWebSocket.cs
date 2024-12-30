using ShareMarket.Core.Hubs.SharekhanHub;
using ShareMarket.Core.Models.Sharekhan;

namespace ShareMarket.Core.Interfaces.Hubs.SharekhanHub;

public interface ISharekhanWebSocket
{
    public List<SharekhanFeedData> Tradings { get; set; }

    bool IsConnected();
    
    void Subscribe(string? accessToken, string? apiKey, List<int> sharekhanSyncCodes);
    
    event EventHandler<SharekhanWebSocketEventArgs>? MessageReceived;
    
    void RunScript(string script);
    
    Task DisConnect(bool abort = false);
}
