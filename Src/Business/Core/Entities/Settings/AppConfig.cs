namespace ShareMarket.Core.Entities.Settings;

public class AppConfig : Auditable
{
    public string           Provider        { get; set; } = default!;
    public string           AppName         { get; set; } = default!;
    public string           RedirectUrl     { get; set; } = default!;
    public string           ApiKey          { get; set; } = default!;
    public string           ApiSecretKey    { get; set; } = default!;
    public string?          AccessToken     { get; set; }
    public DateTimeOffset?  ExpireOn        { get; set; }
}
