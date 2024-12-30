namespace ShareMarket.Core.Entities.Schemes;

public class Scheme : Auditable
{
    public string       FundName            { get; set; } = default!;
    public string       SearchId            { get; set; } = default!;
    public string       Category            { get; set; } = default!;
    public string       SubCategory         { get; set; } = default!;
    public int?         GrowwRating         { get; set; }
    public int          RiskRating          { get; set; }
    public string       SchemeName          { get; set; } = default!;
    public string       SchemeType          { get; set; } = default!;
    public string       FundHouse           { get; set; } = default!;
    public string       SchemeCode          { get; set; } = default!;
    public DateOnly?    LaunchDate          { get; set; }
    public string       Risk                { get; set; } = default!;
    public string       DocType             { get; set; } = default!;
    public string       DirectSchemeName    { get; set; } = default!;
}