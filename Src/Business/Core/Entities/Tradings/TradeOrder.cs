using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Enumerations;

namespace ShareMarket.Core.Entities.Tradings;

public class TradeOrder : Auditable
{
    public string       Code            { get; set; } = default!;
    public decimal      DailyMftInt     { get; set; }
    public long         EquityId        { get; set; }
    public int          HoldingDays     { get; set; }
    public decimal      MarginAmount    { get; set; }
    public decimal      MarginPerc      { get; set; }
    public DateOnly     OrderDate       { get; set; }
    public long?        BuyOrderId      { get; set; }
    public OrderStatus  OrderStatus     { get; set; }
    public decimal      OrderRate       { get; set; }
    public decimal      OrderValue      { get; set; }
    public int          Quantity        { get; set; }
    public string?      Remark          { get; set; }
    public string       Strategy        { get; set; } = default!;
    public long         TradeBookId     { get; set; }
    public SellAction   SellAction      { get; set; }
    public decimal      TargetPerc      { get; set; }
    public TradeType    TradeType       { get; set; }
    public OrderType    OrderType       { get; set; }
    public string       TradingAccount  { get; set; } = default!;

    public EquityStock  Equity          { get; set; } = default!;
    public TradeBook    TradeBook       { get; set; } = default!;
    public TradeOrder?  BuyOrder        { get; set; }
    public List<TradeOrder>? SellOrders { get; set; }
    public decimal GetMarginAmount => (100 - MarginPerc) / 100 * OrderRate * Quantity;
}

