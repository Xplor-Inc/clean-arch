using ShareMarket.Core.Entities.Schemes;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Models.Groww;
using ShareMarket.Core.Models.Services.Groww;
using System.Net;

namespace ShareMarket.Core.Services.Groww;

public class GrowwService : IGrowwService
{
    private static readonly HttpClient Client = new() { BaseAddress = new Uri("https://groww.in/") };
    public async Task<Result<GrowwStockModel?>> GetLTPPrice(string code, CancellationToken cancellationToken = default)
    {
        Result<GrowwStockModel?> result = new(null);
        try
        {
            var response = await Client.GetAsync($"v1/api/stocks_data/v1/accord_points/exchange/NSE/segment/CASH/latest_prices_ohlc/{code}");
        https://groww.in/v1/api/stocks_fo_data/v1/tr_live_prices/exchange/NSE/segment/FNO/MIDCPNIFTY24DEC12750PE/latest
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result.ResultObject = await response.Content.ReadAsAsync<GrowwStockModel>();
            }
            else
            {
                result.AddError($"API Error for Code :{code}, {await response.Content.ReadAsStringAsync()}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at GetLTPPrice: {code} {ex.Message}");
            return result;
        }
        return result;
    }
   
    public async Task<Result<GrowwStockModel?>> GetFnOPrice(string code, CancellationToken cancellationToken = default)
    {
        Result<GrowwStockModel?> result = new(null);
        try
        {
            var response = await Client.GetAsync($"v1/api/stocks_fo_data/v1/tr_live_prices/exchange/NSE/segment/FNO/{code}/latest");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result.ResultObject = await response.Content.ReadAsAsync<GrowwStockModel>();
            }
            else
            {
                result.AddError($"API Error for Code :{code}, {await response.Content.ReadAsStringAsync()}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at GetLTPPrice: {code} {ex.Message}");
            return result;
        }
        return result;
    }

    public async Task<Result<List<GrowwSchemeModel>>> GetSchemes(CancellationToken cancellationToken = default)
    {
        Result<List<GrowwSchemeModel>> result = new([]);
        try
        {
            int size = 15;
            int pageNum = 0;
        FetchMore:
            var response = await Client.GetAsync($"v1/api/search/v1/derived/scheme?available_for_investment=true&category=Equity&" +
                $"doc_type=scheme&lumpsum_allowed=true&max_aum=&page={pageNum}&plan_type=Direct&q=&sip_allowed=true&size={size}&sort_by=3", cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (cancellationToken.IsCancellationRequested) return result;
                var zx = await response.Content.ReadAsStringAsync(cancellationToken);
                var responseData = await response.Content.ReadAsAsync<GrowwSchemeRootModel>(cancellationToken);
                result.ResultObject.AddRange(responseData.Content);
                if (responseData.TotalResults > (pageNum + 1) * size)
                {
                    pageNum++;
                    goto FetchMore;
                }
            }
            else
            {
                result.AddError($"API Error for Code :GetSchemes, {await response.Content.ReadAsStringAsync()}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at GetSchemes: {ex.Message}");
            return result;
        }
        return result;
    }

    public async Task<Result<GrowwSchemeRootModel?>> GetSchemeHoldings(Scheme scheme, CancellationToken cancellationToken = default)
    {
        Result<GrowwSchemeRootModel?> result = new(null);
        try
        {
            string? code = scheme.SearchId;
            int retryCount = 0;
        RetryForSearchId:
            var response = await Client.GetAsync($"/v1/api/data/mf/web/v4/scheme/search/{code}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result.ResultObject = await response.Content.ReadAsAsync<GrowwSchemeRootModel>();
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError && code == scheme.SearchId && retryCount < 1)
                {
                    retryCount++;
                    var x = await Client.GetAsync($"/mutual-funds/{code}");
                    if (x.StatusCode == HttpStatusCode.OK)
                    {
                        code = x.RequestMessage?.RequestUri?.AbsolutePath.Split('/')[^1];
                        if (code is null) return result;
                        code = code.Replace(":", "%3A").Replace("  ", " ").Replace("  ", " ").Replace(" ", "-").ToLower();
                        goto RetryForSearchId;
                    }
                }
                result.AddError($"API Error for Code :{scheme.SearchId}, {await response.Content.ReadAsStringAsync()}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at GetSchemeHoldings: {scheme.SearchId} {ex.Message}");
            return result;
        }
        return result;
    }

    public async Task<Result<List<ScripRecordModel>>> GetScrips(CancellationToken cancellationToken = default)
    {
        Result<List<ScripRecordModel>> result = new([]);
        try
        {
            var searchParam = new ScripSearchRootDto();

        FetchMore:
            var response = await Client.PostAsJsonAsync($"/v1/api/stocks_data/v1/all_stocks", searchParam, cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var model = await response.Content.ReadAsAsync<ScripRootModel>(cancellationToken);
                result.ResultObject.AddRange(model.Records);
                if (model.TotalRecords > (searchParam.Page + 1) * searchParam.Size)
                {
                    searchParam.Page++;
                    goto FetchMore;
                }
            }
            else
            {
                result.AddError($"API Error for Code :{searchParam}, {await response.Content.ReadAsStringAsync()}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at GetScrips: {ex}");
            return result;
        }
        return result;
    }
    
    public async Task<Result<List<GrowwEtf>>> GetETFs(CancellationToken cancellationToken = default)
    {
        Result<List<GrowwEtf>> result = new([]);
        try
        {
            int pageNo = 0;
        FetchMore:
            var response = await Client.GetAsync($"v1/api/stocks_data/v1/etfs?page={pageNo}&size=1000", cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var model = await response.Content.ReadAsAsync<GrowwETFRootModel>(cancellationToken);
                result.ResultObject.AddRange(model.Etfs);
                if (model.Total > (pageNo + 1) * 1000)
                {
                    pageNo++;
                    goto FetchMore;
                }
            }
            else
            {
                result.AddError($"API Error for Code :v1/api/stocks_data/v1/etfs?page={pageNo}&size=1000, {await response.Content.ReadAsStringAsync()}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at GetScrips: {ex}");
            return result;
        }
        return result;
    }
}