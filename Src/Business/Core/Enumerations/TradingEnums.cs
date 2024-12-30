namespace ShareMarket.Core.Enumerations;

public enum SellAction      { Manuall, Stoploss, Target}
public enum BuyStratergy    { RSIBelow35, RSI55To70, RSI14EMADiffLess1 }
public enum TradeType       { Delivery, Intraday, MTF, Options, Future }
public enum OrderType       { Buy, Sell }
public enum EquityType      { Equity, ETF }
public enum OrderStatus     { Open, Close }
