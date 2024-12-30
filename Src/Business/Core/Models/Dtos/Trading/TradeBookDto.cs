using ShareMarket.Core.Enumerations;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Models.Dtos.Equities;

namespace ShareMarket.Core.Models.Dtos.Trading;

public class TradeBookDto : AuditableDto
{
    public decimal      AchivedTarget   { get; set; }
    public decimal      Brokerage       { get; set; }
    public DateOnly?    BuyDate         { get; set; }
    public decimal?     BuyRate         { get; set; }
    public decimal      BuyValue        { get; set; }
    public decimal      DailyMftInt     { get; set; }
    public long         EquityId        { get; set; }
    public int          HoldingDays     { get; set; }
    public decimal      MarginAmount    { get; set; }
    public decimal      MarginPerc      { get; set; }
    public decimal      MarginInterest  { get; set; }
    public decimal      MarketValue     { get; set; }
    public int          Postion         { get; set; }
    public int          Quantity        { get; set; }
    public decimal      ReleasedPL      { get; set; }
    public decimal      ReleasedPLPerc  { get; set; }
    public string?      Remark          { get; set; }
    public SellAction?  SellAction      { get; set; }
    public DateOnly?    SellDate        { get; set; }
    public decimal      SellRate        { get; set; }
    public decimal      SellTarget      { get; set; }
    public decimal      SellValue       { get; set; }
    public string       Code            { get; set; } = default!;
    public string       Strategy        { get; set; } = default!;
    public decimal?     TargetPerc      { get; set; }
    public decimal      TargetRate      { get; set; }
    public TradeType?   TradeType       { get; set; }
    public string       TradingAccount  { get; set; } = default!;

    public decimal      CurrentStatus   => BuyRate == 0 ? 100 : (Equity.LTP - BuyRate ?? 0) / (BuyRate ?? 0) * 100;
    public decimal      ActualPLPerc    => BuyRate == 0 ? 100 : ((MarketValue - BuyValue - MarginInterest) / (BuyValue - MarginAmount) * 100).ToFixed();

    public List<TradeOrderDto>      Orders          { get; set; } = [];
    public EquityStockDto           Equity          { get; set; } = default!;
}
