using Newtonsoft.Json;
using ShareMarket.Core.Entities.ScanX;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Interfaces.Services.ScanX;
using ShareMarket.Core.Models.ScanX;
using System.Net;

namespace ShareMarket.Core.Services.ScanX;

public class ScanXService : IScanXService
{
    private static readonly HttpClient Client = new() { BaseAddress = new Uri("https://ow-scanx-analytics.dhan.co/") };

    public async Task<Result<List<ScanXEquity>>> GetScrips(CancellationToken cancellationToken)
    {
        Result<List<ScanXEquity>> result = new([]);
        try
        {
            var searchParam = new ScanXRequestModel();
            searchParam.Data.Count = 10000;
            var response = await Client.PostAsJsonAsync($"customscan/fetchdt", searchParam, cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                //var xsa = await response.Content.ReadAsStringAsync(cancellationToken);
                var model = await response.Content.ReadAsAsync<ScanXResponseModel>(cancellationToken);
                result.ResultObject.AddRange(model.Data);
            }
            else
            {
                result.AddError($"API Error for Code :{searchParam}, {await response.Content.ReadAsStringAsync(cancellationToken)}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at ScanXService:GetScrips: {ex}");
            return result;
        }
        return result;
    }
    
    public async Task<Result<IndicatorResponseModel>> GetIndicators(ScanXEquity scan, CancellationToken cancellationToken = default)
    {
        Result<IndicatorResponseModel> result = new(default!);
        try
        {
            var searchParam = new IndicatorRequestModel
            {
                Security_Id = scan.Sid.ToFixed(0).ToString(),
                Segment     = scan.Seg,
                Isin        = scan.Isin,
                Exchange    = scan.Exch,
                Symbol      = scan.DispSym
            };

            var response = await Client.PostAsJsonAsync($"https://static-scanx.dhan.co/staticscanx/indicator", searchParam, cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var model = await response.Content.ReadAsAsync<IndicatorResponseModel>(cancellationToken);
                result.ResultObject = model;
            }
            else
            {
                result.AddError($"API Error for Code :{searchParam}, {await response.Content.ReadAsStringAsync(cancellationToken)}");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.AddError($"Exception at ScanXService:GetScrips: {ex}");
            return result;
        }
        return result;
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class IndicatorRequestModel
{
    [JsonProperty("exchange")]
    public string Exchange { get; set; } = string.Empty;

    [JsonProperty("segment")]
    public string Segment { get; set; } = string.Empty;

    [JsonProperty("security_id")]
    public string Security_Id { get; set; } = string.Empty;

    [JsonProperty("isin")]
    public string Isin { get; set; } = string.Empty;

    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("minute")]
    public string Minute { get; set; } = "D";
}
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class PivotData
{
    public double PP { get; set; }

    public double R1 { get; set; }

    public double R2 { get; set; }

    public double R3 { get; set; }

    public double S1 { get; set; }

    public double S2 { get; set; }

    public double S3 { get; set; }
}

public class Datum
{
    public List<IndicatorValue> EMA { get; set; } = [];

    public List<IndicatorValue> Indicator { get; set; } = [];

    public List<IndicatorValue> SMA { get; set; } = [];

    public List<ScanXPivot> Pivot { get; set; } = [];
}

public class IndicatorValue
{
    public string Action { get; set; } = default!;

    public string Indicator { get; set; } = default!;

    public double Value { get; set; }
}

public class ScanXPivot
{
    public PivotData Camarilla { get; set; } = default!;

    public PivotData Classic { get; set; } = default!;

    public PivotData Fibonacci { get; set; } = default!;
}

public class IndicatorResponseModel
{
    public List<Datum> Data { get; set; } = [];

    [JsonProperty("error_code")]
    public int error_code { get; set; }

    public string Message { get; set; } = default!;

    public string Status { get; set; } = default!;
}

