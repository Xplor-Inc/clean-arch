using ShareMarket.Core.Entities;

namespace ShareMarket.WebApp.Components.Pages.Tradings.FnO;

public class OptionTradingDto : Auditable
{
    public string?      Analysis        { get; set; }
    public decimal      Brokerage       { get; set; }
    public DateOnly     BuyDate         { get; set; } 
    public decimal      BuyRate         { get; set; }
    public TradeType    ContractType    { get; set; }
    public DateOnly     ExpireDate      { get; set; } 
    public bool         HasSL           { get; set; }
    public string       Index           { get; set; } = default!;
    public decimal      NetPL           { get; set; }
    public int          OpenPosition    { get; set; }
    public OptionType   OptionType      { get; set; }
    public decimal      PL              { get; set; }
    public int          Quantity        { get; set; }
    public string       Ref             { get; set; } = default!;
    public SellAction   SellAction      { get; set; }
    public DateOnly?    SellDate        { get; set; } 
    public decimal      SellRate        { get; set; }
    public int          StrickPrice     { get; set; }
    public string       ContractId      { get; set; } = default!;
    public OrderStatus OrderStatus { get; set; }

    #region MyRegion
    public decimal GetPLAmount()
    {
        return ((SellRate - BuyRate) * (Quantity - OpenPosition)).ToFixed();
    }
    #region MyRegion
    public decimal GetBuyAmount()
    {
        return (BuyRate * Quantity).ToFixed();
    }
    public decimal GetSellAmount()
    {
            return (SellRate * Quantity).ToFixed();
    }
    #endregion

    #endregion
}

public enum OptionType
{
    Call = 1,
    Put = 2
}

