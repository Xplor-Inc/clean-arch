namespace ShareMarket.Core.Entities.Equities;

public class EquityPriceHistory : Auditable
{
    public decimal  Avg14DaysProfit { get; set; }
    public decimal  Avg14DaysLoss   { get; set; }
    public decimal  Close           { get; set; }
    public string   Code            { get; set; } = default!;
    public DateOnly Date            { get; set; }
    public decimal  DayChange       { get; set; }
    public decimal  DayChangePer    { get; set; }
    public long     EquityId        { get; set; }
    public decimal  High            { get; set; }
    public decimal  Loss            { get; set; }
    public decimal  Low             { get; set; }
    public string   Name            { get; set; } = default!;
    public decimal  Open            { get; set; }
    public decimal  Profit          { get; set; }
    public decimal  RS              { get; set; }
    public decimal  RSI             { get; set; }
    public decimal  RSI14EMA        { get; set; }
    public decimal  RSI14EMADiff    { get; set; }
    public decimal  DMA5            { get; set; }
    public decimal  DMA10           { get; set; }
    public decimal  DMA20           { get; set; }
    public decimal  DMA50           { get; set; }
    public decimal  DMA100          { get; set; }
    public decimal  DMA200          { get; set; }

    public EquityStock Equity       { get; set; } = default!;
}
