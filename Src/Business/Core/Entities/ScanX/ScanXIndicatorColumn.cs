using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.Core.Entities.ScanX;

public class ScanXIndicatorColumn : Auditable
{
    public long         ScanXId       { get; set; } = default!;
    public long?        EquityId      { get; set; }
    public string       Action_1      { get; set; } = default!;
    public string       Indicator_1   { get; set; } = default!;
    public double       Value_1       { get; set; } = default!;
    public string       Action_2      { get; set; } = default!;
    public string       Indicator_2   { get; set; } = default!;
    public double       Value_2       { get; set; } = default!;
    public string       Action_3      { get; set; } = default!;
    public string       Indicator_3   { get; set; } = default!;
    public double       Value_3       { get; set; } = default!;
    public string       Action_4      { get; set; } = default!;
    public string       Indicator_4   { get; set; } = default!;
    public double       Value_4       { get; set; } = default!;
    public string       Action_5      { get; set; } = default!;
    public string       Indicator_5   { get; set; } = default!;
    public double       Value_5       { get; set; } = default!;
    public string       Action_6      { get; set; } = default!;
    public string       Indicator_6   { get; set; } = default!;
    public double       Value_6       { get; set; } = default!;
    public string       Action_7      { get; set; } = default!;
    public string       Indicator_7   { get; set; } = default!;
    public double       Value_7       { get; set; } = default!;
    public string       Action_8      { get; set; } = default!;
    public string       Indicator_8   { get; set; } = default!;
    public double       Value_8       { get; set; } = default!;
    public string       Action_9      { get; set; } = default!;
    public string       Indicator_9   { get; set; } = default!;
    public double       Value_9       { get; set; } = default!;
    public ScanXEquity  ScanX       { get; set; } = default!;
    public EquityStock? Equity      { get; set; }
}