using ShareMarket.Core.Enumerations;
using ShareMarket.Core.Extensions;

namespace ShareMarket.Core.Utilities;

public class BrokarageCharges
{
    public static decimal Groww(decimal amount, OrderType action, TradeType type)
    {
        decimal stt = GetSTT(amount, action, type);
        decimal stampDuty = GetStampDuty(amount, type, action);
        decimal etc = GetExchangeTransCharges(amount, type);
        decimal sebiTurnOver = GetSebiTurnoverCharges(amount);
        decimal dpCharge = GetDPCharges(type, action);
        decimal ipftcharge = GetIPFTCharges(amount, type);
        decimal groww = GetGrowwCharges(amount, type);
        var gst = (groww + dpCharge + etc + ipftcharge + sebiTurnOver) * 18 / 100;
        decimal total = gst + stt + stampDuty + etc + sebiTurnOver + dpCharge + ipftcharge + groww;
        return total;
    }

    private static decimal GetSTT(decimal amount, OrderType action, TradeType tradeType)
    {
        var charge = 0.0M;
        if (tradeType == TradeType.Intraday && action == OrderType.Sell) return (amount * 0.025M / 100).ToFixed();
        if (tradeType == TradeType.Delivery) return (amount * 0.1M / 100).ToFixed();
        if (tradeType == TradeType.Future && action == OrderType.Sell) return (amount * 0.0125M / 100).ToFixed();
        if (tradeType == TradeType.Options && action == OrderType.Sell) return (amount * 0.0625M / 100).ToFixed();
        return charge;
    }
    private static decimal GetStampDuty(decimal amount, TradeType xAction, OrderType tradeAction)
    {
        var charge = 0.0M;
        if (tradeAction == OrderType.Sell) return charge;
        if (xAction == TradeType.Intraday) return (amount * 0.003M / 100).ToFixed();
        if (xAction == TradeType.Delivery) return (amount * 0.015M / 100).ToFixed();
        if (xAction == TradeType.Future) return (amount * 0.002M / 100).ToFixed();
        if (xAction == TradeType.Options) return (amount * 0.003M / 100).ToFixed();
        return charge;
    }
    private static decimal GetExchangeTransCharges(decimal amount, TradeType xAction)
    {
        var charge = 0.0M;
        if (xAction == TradeType.Intraday || xAction == TradeType.Delivery) return (amount * 0.00297M / 100).ToFixed();
        if (xAction == TradeType.Future) return (amount * 0.00188M / 100).ToFixed();
        if (xAction == TradeType.Options) return (amount * 0.0495M / 100).ToFixed();
        return charge;
    }
    private static decimal GetSebiTurnoverCharges(decimal amount)
    {
        return (amount * 0.0001M / 100).ToFixed();
    }
    private static decimal GetDPCharges(TradeType xAction, OrderType action)
    {
        if (xAction == TradeType.Delivery && action == OrderType.Sell) return 18.25M;
        return 0.0M;
    }
    private static decimal GetIPFTCharges(decimal amount, TradeType xAction)
    {
        if (xAction == TradeType.Options) return (amount * 0.0005M / 100).ToFixed();
        return (amount * 0.0001M / 100).ToFixed();
    }
    private static decimal GetGrowwCharges(decimal amount, TradeType xAction)
    {
        var x = (amount * 0.05M / 100).ToFixed();
        if (xAction == TradeType.Future || xAction == TradeType.Options) return 20M;
        return x > 20 ? 20 : x;
    }

}