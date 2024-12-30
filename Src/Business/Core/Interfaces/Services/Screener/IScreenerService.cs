using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Models.Results;

namespace ShareMarket.Core.Interfaces.Services.Screener;

public interface IScreenerService
{
    Task<decimal> SyncShareHoldingByScreener(EquityStock stock);
}