using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.Core.Interfaces.Services.EquityPandit;

public interface IEquityPanditService
{
    Task CreateSyncErrorLog(string code, string provider, string error);
    Task<Result<List<EquityPriceHistory>>> SyncPriceEquityPandit(EquityStock stock);
    Task<Result<EquityStock>> SyncFundamentalEquityPandit(EquityStock stock);
}