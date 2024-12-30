namespace ShareMarket.Core.Models.Configurations;

public class EmailConfiguration
{
    public bool     EnableSsl   { get; set; }
    public string   From        { get; set; } = string.Empty;
    public string   Header      { get; set; } = string.Empty;
    public string   Host        { get; set; } = string.Empty;
    public string   Password    { get; set; } = string.Empty;
    public int      Port        { get; set; }
    public bool     SendEmail   { get; set; }
    public string   ReplyTo     { get; set; } = string.Empty;
    public string   UserName    { get; set; } = string.Empty;
}
public class EmailTemplate
{
    public string UpcomingBillRemindars { get; set; } = string.Empty;
    public string WeeklyStockReport     { get; set; } = string.Empty;
}