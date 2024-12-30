using ShareMarket.Core.Entities.ScanX;

namespace ShareMarket.Core.Interfaces.Conductors.ScanX
{
    public interface IScanXEquityConductor
    {
        Task GetAllStocks();
        Task CreateOrUpdate(ScanXEquity equity);
        Task GetIndicatorData(CancellationToken cancellationToken = default);
        Task GetIndicatorData(ScanXEquity scanX, CancellationToken cancellationToken = default);
        Task GetIndicatorData_Column(ScanXEquity scanX, CancellationToken cancellationToken = default);
    }
}