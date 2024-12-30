namespace ShareMarket.WebApp.Models.Pages.Tradebooks;

public class TradeBookSummaryModel
{
    public decimal Invested             { get; set; }
    public decimal Current              { get; set; }
    public decimal NonReleasedPL        { get; set; }
    public decimal NonReleasedPLPerc    { get; set; }
    public decimal ReleasedPL           { get; set; }
    public decimal ReleasedPLPerc       { get; set; }
    public decimal DayChange            { get; set; }
    public decimal DayChangePerc        { get; set; }
}
