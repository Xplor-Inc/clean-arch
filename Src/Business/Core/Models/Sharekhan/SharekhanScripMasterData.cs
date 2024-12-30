namespace ShareMarket.Core.Models.Sharekhan;

public class SharekhanScripMasterData
{
    public List<SciptMasterDto> Data { get; set; } = [];
}
public class SciptMasterDto
{
    public int ScripCode { get; set; }
    public double TickSize { get; set; }
    public string InstType { get; set; } = string.Empty;
    public int LotSize { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Indices { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string IsinCode { get; set; } = string.Empty;
    public string Expiry { get; set; } = string.Empty;
    public double Strike { get; set; }
    public double Multiplier { get; set; }
    public object OptionType { get; set; } = string.Empty;
    public string TradingSymbol { get; set; } = string.Empty;
}
