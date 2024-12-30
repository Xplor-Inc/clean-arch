using Hangfire;
using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Entities.Schemes;

namespace ShareMarket.Core.Interfaces.Conductors.EquitiesConductors;

public interface IEquityDailyPriceSyncConductor
{

    Task UpdateRank();
    
    [Queue("default")]
    Task<int> SyncScripMasterAsync(CancellationToken cancellationToken);
    
    [Queue("ltp")]
    Task<int> SyncEquityLTPAsync(CancellationToken cancellationToken);
   
    [Queue("default")]
    Task<int> SyncSchemeAsync(CancellationToken cancellationToken);
   
    [Queue("default")]
    Task<int> SyncSchemeHoldingsAsync(Scheme scheme);
 
    [Queue("ltp")]
    Task<string> SyncLtpByStockAsync(string code, DateOnly date, string loop);
   
    [Queue("history")]
    Task<int> SyncHistoryByPandit(CancellationToken cancellationToken);
 
    [Queue("calculation")]
    Task<int> RSI_X_EMA_DMA_Calculation(CancellationToken cancellationToken);
 
    [Queue("default")]
    Task<int> EnqueFundamentalSyncByPandit(CancellationToken cancellationToken);
  
    [Queue("history")]
    Task<int> SyncHistoryByPandit(EquityStock equity, string loop);

    [Queue("default")]
    Task<int> SyncShareHoldingByScreener(CancellationToken cancellationToken);
    
    Task<decimal> SyncShareHoldingByScreener(EquityStock equity);
    
    Task SynchStockListByWatchList();
    
    Task<int> ProcessETFs(CancellationToken cancellationToken = default);
}