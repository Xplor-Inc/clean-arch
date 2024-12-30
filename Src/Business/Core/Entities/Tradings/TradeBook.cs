using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Enumerations;
using ShareMarket.Core.Extensions;

namespace ShareMarket.Core.Entities.Tradings;

public class TradeBook : Auditable
{
    public decimal      AchivedTarget   { get; set; }
    public decimal      Brokerage       { get; set; }
    public DateOnly     BuyDate         { get; set; }
    public decimal      BuyRate         { get; set; }
    public decimal      BuyValue        { get; set; }
    public string       Code            { get; set; } = default!;
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
    public SellAction   SellAction      { get; set; }
    public DateOnly?    SellDate        { get; set; }
    public decimal      SellRate        { get; set; }
    public decimal      SellTarget      { get; set; }
    public decimal      SellValue       { get; set; }
    public string       Strategy        { get; set; } = default!;
    public decimal      TargetPerc      { get; set; }
    public decimal      TargetRate      { get; set; }
    public TradeType    TradeType       { get; set; }
    public string       TradingAccount  { get; set; } = default!;

    public List<TradeOrder>     Orders          { get; set; } = [];
    public EquityStock          Equity          { get; set; } = default!;

    #region Calculations
    public decimal  GetDailyMftInt      => (MarginAmount * SystemConstant.GROWW_MFT_RATE).ToFixed();
    public decimal  GetInvestValue      => BuyRate * Quantity;
    public decimal  GetMarginAmount     => (100 - MarginPerc) / 100 * BuyRate * Quantity;
    public decimal  GetReleasesPL       => ((SellRate - BuyRate) * Quantity) - GetMarginInterest;
    public decimal  GetReleasesPLPerc   => BuyRate == 0 ? 100 : (ReleasedPL / (BuyValue - MarginAmount) * 100).ToFixed();
    public decimal  GetMarginInterest   => (DailyMftInt * HoldingDays).ToFixed();
    public int      GetHoldingDays      => (SellDate ?? DateOnly.FromDateTime(DateTime.Now)).DayNumber - BuyDate.DayNumber + 1;
    #endregion

}
