using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.Core.Entities.Schemes;

public class SchemeEquityHolding : Auditable
{
    public string?  Code            { get; set; }
    public string   CompanyName     { get; set; } = default!;
    public double?  CorpusPer       { get; set; }
    public long?    EquityId        { get; set; }
    public string?  InstrumentName  { get; set; } = default!;
    public string?  MarketCap       { get; set; } = default!;
    public string?  MarketValue     { get; set; } = default!;
    public string?  NatureName      { get; set; } = default!;
    public string?  Rating          { get; set; } = default!;
    public string?  RatingMarketCap { get; set; } = default!;
    public string   SchemeCode      { get; set; } = default!;
    public string?  SectorName      { get; set; } = default!;
    public long?    SchemeId        { get; set; }
    public string   StockSearchId   { get; set; } = default!;

    public EquityStock  Equity { get; set; } = default!;
    public Scheme?      Scheme { get; set; }

}
