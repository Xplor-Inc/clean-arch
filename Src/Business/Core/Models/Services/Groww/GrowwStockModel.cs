using Newtonsoft.Json;

namespace ShareMarket.Core.Models.Services.Groww;

public class GrowwStockModel
{
    public decimal Open             { get; set; }
    public decimal High             { get; set; }
    public decimal Low              { get; set; }
    public decimal Ltp              { get; set; }
    public decimal Close            { get; set; }
    public decimal DayChange        { get; set; }
    public decimal DayChangePerc    { get; set; }
}

public class GrowwSchemeModel
{
    [JsonProperty("fund_name")]
    public string FundName { get; set; } = string.Empty;

    [JsonProperty("search_id")]
    public string SearchId { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    [JsonProperty("sub_category")]
    public string SubCategory { get; set; } = string.Empty;

    [JsonProperty("groww_rating")]
    public int? GrowwRating { get; set; }

    [JsonProperty("risk_rating")]
    public int RiskRating { get; set; }

    [JsonProperty("scheme_name")]
    public string SchemeName { get; set; } = string.Empty;

    [JsonProperty("scheme_type")]
    public string SchemeType { get; set; } = string.Empty;

    [JsonProperty("fund_house")]
    public string FundHouse { get; set; } = string.Empty;

    [JsonProperty("scheme_code")]
    public string SchemeCode { get; set; } = string.Empty;

    [JsonProperty("launch_date")]
    public DateOnly? LaunchDate { get; set; }

    public string Risk { get; set; } = string.Empty;

    [JsonProperty("doc_type")]
    public string DocType { get; set; } = string.Empty;

    [JsonProperty("direct_scheme_name")]
    public string DirectSchemeName { get; set; } = string.Empty;

    [JsonProperty("sip_allowed")]
    public bool SipAllowed { get; set; }
}

public class GrowwSchemeRootModel
{
    public List<GrowwSchemeModel> Content { get; set; } = [];

    [JsonProperty("total_results")]
    public int TotalResults { get; set; }

    [JsonProperty("scheme_code")]
    public string? SchemeCode { get; set; }


    [JsonProperty("scheme_name")]
    public string? SchemeName { get; set; }

    [JsonProperty("search_id")]
    public string? SearchId { get; set; }
    public List<SchemeHoldingModel> Holdings { get; set; } = [];
}

public class SchemeHoldingModel
{
    [JsonProperty("scheme_code")]
    public string? SchemeCode { get; set; }

    [JsonProperty("portfolio_date")]
    public DateTime PortfolioDate { get; set; }

    [JsonProperty("company_name")]
    public string? CompanyName { get; set; }

    [JsonProperty("nature_name")]
    public string? NatureName { get; set; }

    [JsonProperty("sector_name")]
    public string? SectorName { get; set; }

    [JsonProperty("instrument_name")]
    public string? InstrumentName { get; set; }

    [JsonProperty("rating")]
    public string? Rating { get; set; }

    [JsonProperty("market_value")]
    public string? MarketValue { get; set; }

    [JsonProperty("corpus_per")]
    public double? CorpusPer { get; set; }

    [JsonProperty("market_cap")]
    public string? MarketCap { get; set; }

    [JsonProperty("rating_market_cap")]
    public string? RatingMarketCap { get; set; }

    [JsonProperty("stock_search_id")]
    public string? StockSearchId { get; set; }
}