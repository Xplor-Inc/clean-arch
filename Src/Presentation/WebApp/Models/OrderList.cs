namespace ShareMarket.WebApp.Models;

public class OrderList
{
    public decimal          AvgFillPrice { get; set; }
    public required  string BuySell { get; set; }
    public  required string CompanyShortName { get; set; }
    public  required string ContractId { get; set; }
    public DateTime         CreatedAt { get; set; }
    public  required string DisplayName { get; set; }
    public  required string Exchange { get; set; }
    public int              FilledQty { get; set; }
    public  required string GrowwOrderId { get; set; }
    public  required string OrderStatus { get; set; }
    public  required string OrderType { get; set; }
    public int              Price { get; set; }
    public  required string Product { get; set; }
    public int              Qty { get; set; }
    public int              RemainingQty { get; set; }
    public  required string Segment { get; set; }
    public  required string Symbol { get; set; }
    public  required string SymbolName { get; set; }
    public DateTime         TradeDate { get; set; }
    public int              TriggerPrice { get; set; }

}

public class OptionsData
{
    public List<OrderList> OrderList { get; set; }
    public int TotalOpenOrdersCount { get; set; }
}
