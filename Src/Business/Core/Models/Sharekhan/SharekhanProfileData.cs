namespace ShareMarket.Core.Models.Sharekhan;

public class SharekhanProfileData
{
    public string? CustomerId { get; set; }
    public object? UserId { get; set; }
    public string? LoginId { get; set; }
    public string? Token { get; set; }
    public bool BtplusServiceEnabled { get; set; }
    public List<string>? Exchanges { get; set; }
    public string? Broker { get; set; }
    public object? State { get; set; }
    public string? FullName { get; set; }
}
public class SharekhanTokenResult
{
    public bool IsOk { get; set; }
    public int Status { get; set; }
    public string? Message { get; set; }
    public DateTime TimeStamp { get; set; }
    public SharekhanProfileData Data { get; set; } = new();
}
public class SharekhanFeedModelStart
{
    public int Status { get; set; }
    public string Message { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public SharekhanFeedData[] Data { get; set; } = [];
}

public class SharekhanFeedModelContinue
{
    public int Status { get; set; }
    public string Message { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public SharekhanFeedData Data { get; set; } = new SharekhanFeedData();
}

public class SharekhanFeedData
{
    public string ExchangeCode { get; set; } = "";
    public int ScripCode { get; set; }
    public int IdxCode { get; set; }
    public string IdxName { get; set; } = "";
    public string Ltt { get; set; } = "";
    public decimal Ltp { get; set; }
    public decimal PerChange { get; set; }
    public decimal RsChange { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal YrHigh { get; set; }
    public decimal YrLow { get; set; }
    public decimal TurnOver { get; set; }
    public int LastUpdTime { get; set; }
    public int Ltq { get; set; }
    public int Preltq { get; set; }
    public double Settlementprice { get; set; }
    public double Cashturnover { get; set; }
    public double Foturnover { get; set; }
    public double Priceband { get; set; }
}
