using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.Core.Entities.ScanX;

public class ScanXEquity : Auditable
{
    public decimal  BookValue                   { get; set; }
    public decimal  DayRSI14CurrentCandle       { get; set; }
    public decimal  DayVolPrevCandle            { get; set; }
    public string   DispSym                     { get; set; } = string.Empty;
    public decimal  DivYeild                    { get; set; }
    public decimal  Eps                         { get; set; }
    public string   Exch                        { get; set; } = string.Empty;
    public decimal  High1Yr                     { get; set; }
    public decimal  IndPb                       { get; set; }
    public decimal  IndPe                       { get; set; }
    public string   Inst                        { get; set; } = string.Empty;
    public string   Isin                        { get; set; } = string.Empty;
    public decimal  LotSize                     { get; set; }
    public decimal  Low1Yr                      { get; set; }
    public decimal  Ltp                         { get; set; }
    public decimal  Mcap                        { get; set; }
    public decimal  Multiplier                  { get; set; }
    public decimal  NetIncome                   { get; set; }
    public decimal  OCFGrowthOnYr               { get; set; }
    public decimal  PPerchange                  { get; set; }
    public decimal  Pb                          { get; set; }
    public decimal  Pchange                     { get; set; }
    public decimal  Pe                          { get; set; }
    public decimal  PricePerchng1mon            { get; set; }
    public decimal  PricePerchng1week           { get; set; }
    public decimal  PricePerchng1year           { get; set; }
    public decimal  PricePerchng2mon            { get; set; }
    public decimal  PricePerchng2week           { get; set; }
    public decimal  PricePerchng2year           { get; set; }
    public decimal  PricePerchng3mon            { get; set; }
    public decimal  PricePerchng3week           { get; set; }
    public decimal  PricePerchng3year           { get; set; }
    public decimal  PricePerchng4year           { get; set; }
    public decimal  PricePerchng5year           { get; set; }
    public decimal  PricePerchng6mon            { get; set; }
    public decimal  PricePerchng9mon            { get; set; }
    public decimal  QoQEBITDAMarginGrowth       { get; set; }
    public decimal  QoQNetIncomeGrowth          { get; set; }
    public decimal  QoQRevenueGrowth            { get; set; }
    public decimal  QoQRoCE                     { get; set; }
    public decimal  QoQRoE                      { get; set; }
    public decimal  ROCE                        { get; set; }
    public decimal  Roe                         { get; set; }
    public string   Seg                         { get; set; } = string.Empty;
    public string   Sector                      { get; set; } = string.Empty;
    public string   SubSector                   { get; set; } = string.Empty;
    public string   Seosym                      { get; set; } = string.Empty;
    public decimal  Sid                         { get; set; }
    public string   Sym                         { get; set; } = string.Empty;
    public decimal  TickSize                    { get; set; }
    public decimal  Volume                      { get; set; }
    public decimal  Year1CAGREBITDAMarginGrowth { get; set; }
    public decimal  Year1NetIncomeGrowth        { get; set; }
    public decimal  Year1ROCE                   { get; set; }
    public decimal  Year1ROE                    { get; set; }
    public decimal  Year1RevenueGrowth          { get; set; }
    public decimal  Year3CAGREBITDAMarginGrowth { get; set; }
    public decimal  Year3CAGRRevenueGrowth      { get; set; }
    public decimal  Year3NetIncomeGrowth        { get; set; }
    public decimal  Year3OperatingCashFlow      { get; set; }
    public decimal  Year3ROCE                   { get; set; }
    public decimal  Year3ROE                    { get; set; }
    public decimal  Year5CAGREBITDAMarginGrowth { get; set; }
    public decimal  Year5CAGRRevenueGrowth      { get; set; }
    public decimal  Year5NetIncomeGrowth        { get; set; }
    public decimal  Year5OperatingCashFlow      { get; set; }
    public decimal  Year5ROCE                   { get; set; }
    public decimal  Year5ROE                    { get; set; }
    public decimal  YearlyRevenue               { get; set; }
    public decimal  YoYLastQtrlyProfitGrowth    { get; set; }
    public long?    EquityId                    { get; set; }

    public EquityStock?         Equity      { get; set; }
    public List<ScanXIndicator> Indicators  { get; set; } = [];

    public void CopyFrom(ScanXEquity source)
    {
        if (source == null) return;

        foreach (var property in typeof(ScanXEquity).GetProperties())
        {
            if (property.CanWrite && !Skipable.Contains(property.Name))
            {
                property.SetValue(this, property.GetValue(source));
            }
        }
    }

    public static string[] Skipable => ["Equity", "EquityId", "UpdatedById", "UpdatedOn", "DeletedById", "DeletedOn", "Id", "CreatedById", "CreatedOn", "CreatedBy", "DeletedBy", "UpdatedBy"];
}
