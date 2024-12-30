using Newtonsoft.Json;

namespace ShareMarket.Core.Models.Groww;

public class LivePriceDto
{
    public decimal Close            { get; set; }
    public decimal High             { get; set; }
    public decimal Low              { get; set; }
    public decimal Ltp              { get; set; }
    public decimal DayChange        { get; set; }
    public decimal DayChangePerc    { get; set; }
}

public class ScripRecordModel
{
    public int?         IndustryCode    { get; set; }
    public string       CompanyName     { get; set; } = default!;
    public string       SearchId        { get; set; } = default!;
    public string?      BseScriptCode   { get; set; } = default!;
    public string?      NseScriptCode   { get; set; } = default!;
    public long         MarketCap       { get; set; } = default!;
    public string       Isin            { get; set; } = default!;
    public LivePriceDto LivePriceDto    { get; set; } = default!;
}

public class ScripRootModel
{
    public List<ScripRecordModel>   Records         { get; set; } = [];
    public int                      TotalRecords    { get; set; }
}

public class ScripRangeFilter
{
    [JsonProperty("max")]
    public long Max { get; set; }

    [JsonProperty("min")]
    public int  Min { get; set; }
}

public class ScripListFilters
{
    [JsonProperty("INDUSTRY")]
    public List<object> Industry    { get; set; } = [];

    [JsonProperty("INDEX")]
    public List<object> Index       { get; set; } = [];
}

public class ScripObjectFilters
{
    [JsonProperty("CLOSE_PRICE")]
    public ScripRangeFilter ClosePrice  { get; set; } = new ScripRangeFilter { Max = 100000, Min = 0 };

    [JsonProperty("MARKET_CAP")]
    public ScripRangeFilter MarketCap   { get; set; } = new ScripRangeFilter { Min = 0, Max = 10000000000000000 };
}

public class ScripSearchRootDto(int page = 0, int size = 10000)
{
    [JsonProperty("page")]
    public int      Page        { get; set; } = page;

    [JsonProperty("size")]
    public int      Size        { get; set; } = size;

    [JsonProperty("sortBy")]
    public string   SortBy      { get; set; } = "MARKET_CAP";

    [JsonProperty("sortType")]
    public string   SortType    { get; set; } = "DESC";
    
    [JsonProperty("listFilters")]
    public ScripListFilters     ListFilters { get; set; } = new ScripListFilters();

    [JsonProperty("objFilters")]
    public ScripObjectFilters   ObjFilters  { get; set; } = new ScripObjectFilters();

}

public class GrowwIndustry
{
    public int      Id      { get; set; }
    public string   Name    { get; set; } = default!;
}

public class GrowwSectorModel
{
    public string               Sector      { get; set; } = default!;
    public List<GrowwIndustry>  Industries  { get; set; } = [];
}
