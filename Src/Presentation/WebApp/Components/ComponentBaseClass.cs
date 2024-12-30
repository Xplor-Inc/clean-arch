using Microsoft.AspNetCore.SignalR.Client;
using ShareMarket.Core.Entities.Settings;
using ShareMarket.Core.Interfaces.Hubs;
using ShareMarket.Core.Interfaces.Hubs.SharekhanHub;
using ShareMarket.Core.Interfaces.Utility.Security;

namespace ShareMarket.WebApp.Components;

public class ComponentBaseClass : ComponentBase
{
    [Inject] NavigationManager Navigation { get; set; } = default!;
    [Inject] INotificationHub Hub { get; set; } = default!;
    [Inject] internal IToastService         NotificationService { get; set; } = default!;
    [Inject] internal IMapper               Mapper              { get; set; } = default!;
    [Inject] private IUserIdentityProcessor UserIdentity        { get; set; } = default!;
    [Inject] internal IMessageService       MessageService      { get; set; } = default!;
    [Inject] protected ISharekhanWebSocket SharekhanWebSocket  { get; set; } = default!;
    [Inject] IRepositoryConductor<AppConfig> AppConfigRepo      { get; set; } = default!;

    private HubConnection? hubConnection;
    internal string? Message { get; set; }
    internal long   UserId      { get; set; }
    internal bool   IsLoading   { get; set; }
    public const string Red = "#ff0000";
    public const string Green = "#008000";

    protected override async Task OnInitializedAsync()
    {
        UserId = await UserIdentity.GetCurrentUserId();
        await base.OnInitializedAsync();
        hubConnection = new HubConnectionBuilder()
        .WithUrl(Navigation.ToAbsoluteUri("/notifications-hub"))
            .Build();

        hubConnection.On<string, string>("Calculations", (user, message) =>
        {
            Message = $"{user}{message}";
            InvokeAsync(StateHasChanged);
        });
        hubConnection.On<string, string>("WatchlistSync", (user, message) =>
        {
            Message = $"{user}{message}";
            InvokeAsync(StateHasChanged);
        });
        await hubConnection.StartAsync();
    }

    protected async Task ConfigureWebSocket(List<int> sharekhanSyncCodes)
    {
        if (sharekhanSyncCodes.Count > 0)
        {
            var path = Navigation.BaseUri;

            var appConfig = await AppConfigRepo.FindAll(x => x.Provider == "Sharekhan" && x.RedirectUrl.StartsWith(path) && x.DeletedOn == null)
                                .ResultObject.FirstOrDefaultAsync();
            if (appConfig is not null && !string.IsNullOrEmpty(appConfig.AccessToken))
            {
                SharekhanWebSocket.Subscribe(appConfig.AccessToken, appConfig.ApiKey, sharekhanSyncCodes);
            }
        }
    }
}
