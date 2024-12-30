using Hangfire;
using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.Core.Interfaces.Conductors.EquitiesConductors;

public interface IEquityTechnicalCalulationConductor
{
    Task<Result<bool>> CreateOrUpdateHistoryAsync(List<EquityPriceHistory> histories);
    
    [Queue("calculation")]
    Task<Result<bool>> SyncEquietyStockCalculation(EquityStock EquityStock, string message = "");
}