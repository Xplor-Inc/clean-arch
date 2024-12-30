using Newtonsoft.Json;

namespace ShareMarket.Core.Models.Groww;

public class GrowwETFRootModel
{
    [JsonProperty("etfs")]
    public List<GrowwEtf> Etfs { get; set; } = [];

    [JsonProperty("total")]
    public int Total { get; set; }
}

public class GrowwETFCompanyHeader
{
    [JsonProperty("searchId")]
    public string SearchId { get; set; } = string.Empty;

    [JsonProperty("growwCompanyId")]
    public string GrowwCompanyId { get; set; } = string.Empty;

    [JsonProperty("isin")]
    public string Isin { get; set; } = string.Empty;

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("shortName")]
    public string ShortName { get; set; } = string.Empty;

    [JsonProperty("nseScriptCode")]
    public string NseScriptCode { get; set; } = string.Empty;

    [JsonProperty("bseScriptCode")]
    public string BseScriptCode { get; set; } = string.Empty;

    [JsonProperty("nseTradingSymbol")]
    public string NseTradingSymbol { get; set; } = string.Empty;

    [JsonProperty("bseTradingSymbol")]
    public string BseTradingSymbol { get; set; } = string.Empty;

    [JsonProperty("isBseTradable")]
    public bool IsBseTradable { get; set; }

    [JsonProperty("isNseTradable")]
    public bool IsNseTradable { get; set; }

    [JsonProperty("logoUrl")]
    public string LogoUrl { get; set; } = string.Empty;

    [JsonProperty("benchmark")]
    public string Benchmark { get; set; } = string.Empty;

    [JsonProperty("isBseFnoEnabled")]
    public bool IsBseFnoEnabled { get; set; }

    [JsonProperty("isNseFnoEnabled")]
    public bool IsNseFnoEnabled { get; set; }

    [JsonProperty("industryId")]
    public int? IndustryId { get; set; }
}

public class GrowwEtf
{
    [JsonProperty("companyHeader")]
    public GrowwETFCompanyHeader CompanyHeader { get; set; } = new();

    [JsonProperty("nseYearLow")]
    public decimal NseYearLow { get; set; }

    [JsonProperty("nseYearHigh")]
    public decimal NseYearHigh { get; set; }

    [JsonProperty("bseYearLow")]
    public decimal BseYearLow { get; set; }

    [JsonProperty("bseYearHigh")]
    public decimal BseYearHigh { get; set; }

    [JsonProperty("expenseRatio")]
    public decimal ExpenseRatio { get; set; }

    [JsonProperty("aum")]
    public decimal Aum { get; set; }
}
