using ShareMarket.Core.Enumerations;
using ShareMarket.Core.Models.Dtos.Equities;

namespace ShareMarket.Core.Models.Dtos.Trading;

public class TradeOrderDto : AuditableDto
{
    public string       Code            { get; set; } = default!;
    public decimal      DailyMftInt     { get; set; }
    public long         EquityId        { get; set; }
    public int          HoldingDays     { get; set; }
    public decimal      MarginAmount    { get; set; }
    public decimal      MarginPerc      { get; set; }
    public DateOnly?    OrderDate       { get; set; }
    public long?        BuyOrderId      { get; set; }
    public decimal?     OrderRate       { get; set; }
    public decimal      OrderValue      { get; set; }
    public int          Quantity        { get; set; }
    public string?      Remark          { get; set; }
    public string       Strategy        { get; set; } = default!;
    public SellAction?  SellAction      { get; set; }
    public OrderStatus  OrderStatus     { get; set; }
    public decimal?     TargetPerc      { get; set; }
    public long         TradeBookId     { get; set; }
    public TradeType?   TradeType       { get; set; }
    public OrderType    OrderType       { get; set; }
    public string       TradingAccount  { get; set; } = default!;
    public int          AvailableToSell { get; set; }
    public DateOnly?    MinSellDate     { get; set; }
    
    public EquityStockDto       Equity          { get; set; } = default!;
    public TradeBookDto         TradeBook       { get; set; } = default!;
    public List<TradeOrderDto>  SellOrders      { get; set; } = [];
}
