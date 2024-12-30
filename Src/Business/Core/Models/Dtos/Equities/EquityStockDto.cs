using ShareMarket.Core.Entities;
using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.Core.Models.Dtos.Equities;

public class EquityStockDto : AuditableDto
{
    public string Code { get; set; } = default!;
    public decimal DayChange { get; set; }
    public decimal DayChangePer { get; set; }
    public decimal DayHigh { get; set; }
    public decimal DayLow { get; set; }
    public int GrowwRank { get; set; }
    public string? GrowwSearchId { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; }
    public decimal LTP { get; set; }
    public DateOnly LTPDate { get; set; }
    public string Name { get; set; } = default!;
    public string? Sector { get; set; }
    public int SharekhanId { get; set; }


    public string ScreenerUrl { get; set; } = string.Empty;
    public string FinologyUrl { get; set; } = string.Empty;
    public string EquityPanditUrl { get; set; } = string.Empty;
    public string? BSECode { get; set; }
    public DateTime? LastSyncOn { get; set; }

    #region Fundamental
    public decimal EPS { get; set; }
    public decimal PE { get; set; }
    public decimal PD { get; set; }
    public decimal Dividend { get; set; }
    public decimal MarketCap { get; set; }
    public decimal FaceValue { get; set; }
    public decimal BookValue { get; set; }
    public decimal DebtEquity { get; set; }
    public decimal ROE { get; set; }
    #endregion

    #region Stratergy


    public string? LongTerm { get; set; }
    public string? MediumTerm { get; set; }
    public string? ShortTerm { get; set; }
    public decimal PromotersHolding { get; set; }
    public decimal MutualFundHoldings { get; set; }

    #endregion


    public List<EquityPriceHistory>? PriceHistories { get; set; }
    public List<WatchlistStock>? WatchlistStockList { get; set; }
    public long? EquityStockCalculationId { get; set; }
    public EquityStockCalculation? EquityStockCalculation { get; set; }
}