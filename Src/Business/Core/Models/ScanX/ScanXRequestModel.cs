namespace ShareMarket.Core.Models.ScanX;

public class ScanXRequestData
{
    public string Sort { get; set; } = "desc";
    public string Sorder { get; set; } = "Mcap";
    public int Count { get; set; } = 10000;
    public List<ScanXRequestParam> Params { get; set; } = [];
    public List<string> Fields { get; set; } = ["DispSym","Ltp","Pchange","PPerchange","Volume","Pe","Mcap","DayRSI14CurrentCandle","Sym"];
    public int Pgno { get; set; } = 1;
}

public class ScanXRequestParam
{
    public string Field { get; set; } = string.Empty;
    public string Op { get; set; } = string.Empty;
    public string Val { get; set; } = string.Empty;
}

public class ScanXRequestModel
{
    public ScanXRequestData Data { get; set; } = new ScanXRequestData
    {
        Count = 25,
        Fields = ["Sector","SubSector",
  "DayVolPrevCandle","Sym",
  "PPerchange",
  "PricePerchng1week",
  "DispSym",
  "Ltp",
  "Mcap",
  "Pe",
  "DivYeild",
  "Pb",
  "Roe",
  "ROCE",
  "Ind_Pe",
  "Ind_Pb",
  "High1Yr",
  "Low1Yr",
  "BookValue",
  "Eps",
  "PricePerchng1mon",
  "PricePerchng2mon",
  "PricePerchng3mon",
  "PricePerchng6mon",
  "PricePerchng9mon",
  "PricePerchng1week",
  "PricePerchng2week",
  "PricePerchng3week",
  "PricePerchng1year",
  "PricePerchng2year",
  "PricePerchng3year",
  "PricePerchng4year",
  "PricePerchng5year",
  "QoQRevenueGrowth",
  "NetIncome",
  "YearlyRevenue",
  "Year1RevenueGrowth",
  "Year3CAGRRevenueGrowth",
  "Year5CAGRRevenueGrowth",
  "QoQNetIncomeGrowth",
  "Year1NetIncomeGrowth",
  "Year3NetIncomeGrowth",
  "Year5NetIncomeGrowth",
  "OCFGrowthOnYr",
  "Year3OperatingCashFlow",
  "Year5OperatingCashFlow",
  "QoQRoE",
  "Year1ROE",
  "Year3ROE",
  "Year5ROE",
  "QoQRoCE",
  "Year1ROCE",
  "Year3ROCE",
  "Year5ROCE",
  "QoQEBITDAMarginGrowth",
  "Year1CAGREBITDAMarginGrowth",
  "Year3CAGREBITDAMarginGrowth",
  "Year5CAGREBITDAMarginGrowth",
  "YoYLastQtrlyProfitGrowth",
  "DayRSI14CurrentCandle"
],
        Pgno = 1,
        Sorder = "desc",
        Sort = "Mcap",
        Params = [ new ScanXRequestParam { Field = "DayRSI14CurrentCandle" ,Op = "gt", Val = "0" },
                    new() { Field = "OgInst" ,Op = "", Val = "ES" },
                    new() { Field = "Volume" ,Op = "gte", Val = "0" }]
    };
}


