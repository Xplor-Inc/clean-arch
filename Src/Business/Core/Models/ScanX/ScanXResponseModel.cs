using Newtonsoft.Json;
using ShareMarket.Core.Entities.ScanX;

namespace ShareMarket.Core.Models.ScanX;

public class ScanXResponseModel
{
    public int Code { get; set; }
    public string Remarks { get; set; } = string.Empty;

    [JsonProperty("tot_rec")]
    public int TotRec { get; set; }

    [JsonProperty("tot_pg")]
    public int TotPg { get; set; }

    public List<ScanXEquity> Data { get; set; } = [];
}
