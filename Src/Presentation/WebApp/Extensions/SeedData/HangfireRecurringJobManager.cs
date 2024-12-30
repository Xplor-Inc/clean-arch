using Hangfire;
using ShareMarket.Core.Conductors.TradeBooks;
using ShareMarket.Core.Interfaces.Conductors.EquitiesConductors;
using ShareMarket.Core.Interfaces.Conductors.ScanX;

namespace ShareMarket.WebApp.Extensions.SeedData;

public static class HangfireRecurringJobManager
{
    public static void CreateRecurringJob()
    {
        var option = new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"), MisfireHandling = MisfireHandlingMode.Ignorable };
        
        RecurringJob.AddOrUpdate<IScanXEquityConductor>("ScanX_GetScrips",                          x => x.GetAllStocks(),                                        "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IScanXEquityConductor>("ScanX_GetIndicator",                       x => x.GetIndicatorData(CancellationToken.None),                "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("Groww_SyncSchemes",               x => x.SyncSchemeAsync(CancellationToken.None),                 "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("Groww_SyncByCode",                x => x.SyncEquityLTPAsync(CancellationToken.None), "0 0 1 11 5", options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("EquityPandit_SyncHistory",        x => x.SyncHistoryByPandit(CancellationToken.None),             "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("Calculation_RSI_X_EMA_DMA",       x => x.RSI_X_EMA_DMA_Calculation(CancellationToken.None),       "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("EquityPandit_SyncFundamental",    x => x.EnqueFundamentalSyncByPandit(CancellationToken.None),    "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("Groww_SyncScripMaster",           x => x.SyncScripMasterAsync(CancellationToken.None),            "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("Screener_SyncShareHoldingByScreener", x => x.SyncShareHoldingByScreener(CancellationToken.None),      "0 0 1 11 5",   options: option);
        RecurringJob.AddOrUpdate<IEquityDailyPriceSyncConductor>("Groww_SyncGetETFs",               x => x.ProcessETFs(CancellationToken.None), "0 0 1 11 5", options: option);
        RecurringJob.AddOrUpdate<ITradeBookConductor>("TradeBook_AggregateTradeBook",                   x => x.AggregateTradeBookAsync(CancellationToken.None), "0 0 1 11 5", options: option);
    }
}