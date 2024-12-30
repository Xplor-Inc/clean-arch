using ShareMarket.Core.Entities.ScanX;
using ShareMarket.Core.Enumerations;

namespace ShareMarket.Core.Entities.Equities;

public class EquityStock : Auditable
{
    public string       Code            { get; set; } = default!;
    public decimal      DayChange       { get; set; }
    public decimal      DayChangePer    { get; set; }
    public decimal      DayHigh         { get; set; }
    public decimal      DayLow          { get; set; }
    public int          GrowwRank       { get; set; }
    public string?      GrowwSearchId   { get; set; }
    public string?      Industry        { get; set; }
    public bool         IsActive        { get; set; }
    public decimal      LTP             { get; set; }
    public DateOnly     LTPDate         { get; set; }
    public string       Name            { get; set; } = default!;
    public string?      Sector          { get; set; }
    public int          SharekhanId     { get; set; }
    public string       ScreenerUrl     { get; set; } = "";
    public string       FinologyUrl     { get; set; } = "";
    public string       EquityPanditUrl { get; set; } = string.Empty;
    public string?      BSECode         { get; set; }
    public DateTime?    LastSyncOn      { get; set; }
    public EquityType   Type            { get; set; }

    #region Fundamental
    public decimal      EPS             { get; set; }
    public decimal      PE              { get; set; }
    public decimal      PD              { get; set; }
    public decimal      Dividend        { get; set; }
    public decimal      MarketCap       { get; set; }
    public decimal      FaceValue       { get; set; }
    public decimal      BookValue       { get; set; }
    public decimal      DebtEquity      { get; set; }
    public decimal      ROE             { get; set; }
    public string       Isin            { get; set; } = string.Empty;

    #endregion

    #region Stratergy


    public string?      LongTerm        { get; set; }
    public string?      MediumTerm      { get; set; }
    public string?      ShortTerm       { get; set; }
    public decimal      PromotersHolding    { get; set; }
    public decimal      MutualFundHoldings  { get; set; }

    #endregion

    public ScanXEquity?                 ScanX                       { get; set; }
    public List<ScanXIndicator>         Indicators                  { get; set; } = [];
    public List<EquityPriceHistory>?    PriceHistories              { get; set; }
    public List<WatchlistStock>?        WatchlistStockList          { get; set; }
    public long?                        EquityStockCalculationId    { get; set; }
    public EquityStockCalculation?      EquityStockCalculation      { get; set; }

    public string GetEquityPanditUrl => $"https://www.equitypandit.com/share-price/{BSECode ?? Code}";
}