using ShareMarket.Core.Entities.ScanX;
using ShareMarket.Core.Services.ScanX;

namespace ShareMarket.Core.Interfaces.Services.ScanX;

public interface IScanXService
{
    Task<Result<List<ScanXEquity>>> GetScrips(CancellationToken cancellationToken);
    Task<Result<IndicatorResponseModel>> GetIndicators(ScanXEquity scan, CancellationToken cancellationToken = default);
}