using ShareMarket.Core.Entities.Schemes;
using ShareMarket.Core.Models.Groww;
using ShareMarket.Core.Models.Services.Groww;

namespace ShareMarket.Core.Interfaces.Services.Groww;

public interface IGrowwService
{
    Task<Result<GrowwStockModel?>> GetLTPPrice(string code, CancellationToken cancellationToken = default);
    
    Task<Result<GrowwSchemeRootModel?>> GetSchemeHoldings(Scheme scheme, CancellationToken cancellationToken = default);
    
    Task<Result<List<GrowwSchemeModel>>> GetSchemes(CancellationToken cancellationToken = default);
    
    Task<Result<List<ScripRecordModel>>> GetScrips(CancellationToken cancellationToken = default);
    
    Task<Result<List<GrowwEtf>>> GetETFs(CancellationToken cancellationToken = default);

    Task<Result<GrowwStockModel?>> GetFnOPrice(string code, CancellationToken cancellationToken = default);
}