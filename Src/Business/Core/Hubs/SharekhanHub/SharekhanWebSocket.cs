using Newtonsoft.Json;
using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Interfaces.Hubs.SharekhanHub;
using ShareMarket.Core.Models.Sharekhan;
using System.Net.WebSockets;
using Websocket.Client;

namespace ShareMarket.Core.Hubs.SharekhanHub;

public class SharekhanWebSocketEventArgs : EventArgs
{
    public string? Message { get; set; }
}

public class SharekhanWebSocket : ISharekhanWebSocket
{
    public List<SharekhanFeedData> Tradings { get; set; } = [];
    readonly ManualResetEvent receivedEvent = new(false);
    int receivedCount = 0;
    WebsocketClient websocketClient = default!;

    public event EventHandler<SharekhanWebSocketEventArgs>? MessageReceived;
    public bool IsConnected()
    {
        if (websocketClient is null)
            return false;

        return websocketClient.IsStarted;
    }
   
    public void Subscribe(string? accessToken, string? apiKey, List<int> sharekhanSyncCodes)
    {
        try
        {
            var receivedEvent = new ManualResetEvent(false);

            string finalurl = $"wss://stream.sharekhan.com/skstream/api/stream?ACCESS_TOKEN={accessToken}&API_KEY={apiKey}";
            var url = new Uri(finalurl);
            var clientWebSocket = new ClientWebSocket();
            clientWebSocket.Options.KeepAliveInterval = TimeSpan.FromHours(1);
            websocketClient = new WebsocketClient(url, () => { return clientWebSocket; });
            websocketClient.MessageReceived.Subscribe((ResponseMessage msg) => Receive(msg.Text));
            websocketClient.Start();

            List<string> nseCode = [];
            foreach (var code in sharekhanSyncCodes)
            {
                nseCode.Add($"\"NC{code}\"");
            }
            string xA = string.Join(",", nseCode);
            string feedData = "{\"action\":\"feed\",\"key\":[\"ltp\"],\"value\":[XXXX]}";
            feedData = feedData.Replace("XXXX", xA);
            RunScript(feedData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message} from ConnectforLiveData method");
            throw;
        }
    }

    public void Receive(string? message)
    {
        if (message == "heartbeat") return;
        SharekhanWebSocketEventArgs args = new()
        {
            Message = message
        };
        if (!string.IsNullOrWhiteSpace(message) && (message.Contains("indices", StringComparison.OrdinalIgnoreCase)
            || message.Contains("feed", StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                var feedModel = JsonConvert.DeserializeObject<SharekhanFeedModelContinue>(message);
                if (feedModel?.Data == null) return;
                var data = feedModel.Data;

                Tradings.RemoveAll(x => x.ScripCode == feedModel.Data.ScripCode);
                Tradings.Add(data);
            }
            catch (Exception) 
            {
                try
                {
                    var feedModel = JsonConvert.DeserializeObject<SharekhanFeedModelStart>(message);
                    if (feedModel?.Data == null || feedModel == null) return;

                    Tradings = [.. feedModel.Data];
                }
                catch (Exception) { }
            }
        }
       
        MessageReceived?.Invoke(this, args);
        receivedCount++;
        if (receivedCount >= 10)
            receivedEvent.Set();
    }
    
    public void HeartBeat(string accessToken)
    {
        string hbmsg = "heartbeat";
        if (websocketClient?.IsStarted ?? false)
        {
            try
            {
                websocketClient.Send(hbmsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} from HeartBeat method");
                throw;
            }
        }
    }

    public void RunScript(string script)
    {
        if (websocketClient.IsStarted)
        {
            try
            {
                websocketClient.Send(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} from RunScript method");
                throw;
            }
        }
    }

    public void Send(string message)
    {
        if (websocketClient.IsStarted)
        {
            try
            {
                websocketClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} from Send method");
                throw;
            }
        }
    }
    
    public async Task DisConnect(bool abort = false)
    {
        if (websocketClient.IsRunning )
        {
            if (abort)
               await websocketClient.Stop(WebSocketCloseStatus.NormalClosure, "Close");
            else
            {
                websocketClient.Dispose();
            }
        }
    }
}
