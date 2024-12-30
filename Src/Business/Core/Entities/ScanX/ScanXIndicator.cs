using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.Core.Entities.ScanX;

public class ScanXIndicator : Auditable
{
    public long         ScanXId     { get; set; } = default!;
    public long?        EquityId    { get; set; }
    public string       Action      { get; set; } = default!;
    public string       Indicator   { get; set; } = default!;
    public string       Group       { get; set; } = default!;
    public double       Value       { get; set; } = default!;
    public ScanXEquity  ScanX       { get; set; } = default!;
    public EquityStock? Equity      { get; set; }
}