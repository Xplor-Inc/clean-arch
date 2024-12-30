using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ShareMarket.Core.Conductors.ScanX;
using ShareMarket.Core.Conductors.TradeBooks;
using ShareMarket.Core.Hubs;
using ShareMarket.Core.Hubs.SharekhanHub;
using ShareMarket.Core.Interfaces.Conductors.ScanX;
using ShareMarket.Core.Interfaces.Hubs;
using ShareMarket.Core.Interfaces.Hubs.SharekhanHub;
using ShareMarket.Core.Interfaces.Services.ScanX;
using ShareMarket.Core.Interfaces.Services.Sharekhan;
using ShareMarket.Core.Services.ScanX;
using ShareMarket.Core.Services.Sharekhan;
using ShareMarket.Core.Utilities.Security;

namespace ShareMarket.Core.Extensions.Startup;
public static class IServiceColletionsStartup
{
    public static void AddUtilityResolver(this IServiceCollection services)
    {
        services.AddScoped<IEncryption,                         Encryption>();
        services.AddSingleton<IHttpContextAccessor,             HttpContextAccessor>();
        services.AddScoped<IEmailClient,                        EmailClient>();
        services.AddScoped<IEquityTechnicalCalulationConductor, EquityTechnicalCalulationConductor>();
        services.AddScoped<IGrowwService,                       GrowwService>();
        services.AddScoped<IEquityDailyPriceSyncConductor,      EquityDailyPriceSyncConductor>();
        services.AddScoped<IEquityPanditService,                EquityPanditService>();
        services.AddScoped<IScreenerService,                    ScreenerService>();
        services.AddScoped<INotificationHub,                    NotificationHub>();
        services.AddScoped<IScanXService,                       ScanXService>();
        services.AddScoped<ITradeBookConductor,                 TradeBookConductor>();
        services.AddScoped<ISharekhanService,                   SharekhanService>();
        services.AddScoped<ISharekhanWebSocket,                 SharekhanWebSocket>();
        services.AddScoped<IScanXEquityConductor,               ScanXEquityConductor>();

        #region Repository Conductor
        services.AddScoped(typeof(IRepositoryConductor<>), typeof(RepositoryConductor<>));
        #endregion
    }
}