namespace ShareMarket.Core.Entities.Equities;

public class EquityHistorySyncLog : Auditable
{
    public string Name          { get; set; } = default!;
    public string Code          { get; set; } = default!;
    public string ErrorMessage  { get; set; } = default!;
    public string Provider      { get; set; } = default!;
}