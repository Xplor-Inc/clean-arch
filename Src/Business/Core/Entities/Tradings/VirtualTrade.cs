using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareMarket.Core.Entities.Tradings;

public class VirtualTrade : Auditable
{
    public DateOnly     BuyDate     { get; set; }
    public decimal      BuyRate     { get; set; }
    public decimal      BuyValue    { get; set; }
    public string       Code        { get; set; } = default!;
    public long         EquityId    { get; set; }
    public decimal      Holding     { get; set; }
    public decimal      LTP         { get; set; }
    public string       Name        { get; set; } = default!;
    public decimal      ReleasedPL  { get; set; }
    public int          Quantity    { get; set; }
    public SellAction   SellAction  { get; set; }
    public DateOnly?    SellDate    { get; set; }
    public decimal      SellRate    { get; set; }
    public decimal      SellValue   { get; set; }
    public decimal      StopLoss    { get; set; }
    public BuyStratergy Stratergy   { get; set; }
    public decimal      Target      { get; set; }
    public decimal      TargetPer   { get; set; }

    [NotMapped]
    public int HoldingOnBuy { get; set; }
    [NotMapped]
    public int HoldingOnSell { get; set; }
    public EquityStock  Equity      { get; set; } = default!;
}