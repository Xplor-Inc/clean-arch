using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Entities.Tradings;
using ShareMarket.Core.Models.Dtos.Equities;

namespace ShareMarket.WebApp.Components.Pages.EquityMarkets;

public partial class EquityMarket
{
    protected List<EquityPriceHistoryDto>   TodaysTrades    { get; set; } = [];
    protected BuyStratergy                  Stratergy       { get; set; }
    protected DateOnly                      Date            { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(0));
    protected string?                       SelectedSector  { get; set; }
    protected int                           GrowwRank       { get; set; } = 50;
    public string?                          Code            { get; set; }
    protected EquityPriceHistoryDto?        BuyStock        { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await GetDataAsync(BuyStratergy.RSIBelow35);
        IsLoading = false;
        await base.OnInitializedAsync();
    }

    protected async Task LTPUpdate()
    {
        IsLoading = true;
        var syncResult = await GrowwService.GetScrips(CancellationToken.None);
        if (syncResult.HasErrors)
        {
            await NotificationService.Error($"Error : {syncResult.GetErrors()}", "Error");
            IsLoading = false;
            return;
        }
        var scrpis = syncResult.ResultObject;
        foreach (var item in TodaysTrades)
        {
            var stock = scrpis.Find(x => x.NseScriptCode == item.Code || x.BseScriptCode == item.Code);
            if (stock != null)
            {
                item.Equity.LTP             = stock.LivePriceDto.Ltp;
                item.Equity.DayChangePer    = stock.LivePriceDto.DayChangePerc;
            }
        }
        IsLoading = false;
    }

    protected async Task GetDataAsync(BuyStratergy buyStratergy)
    {
        IsLoading = true;
        Stratergy = buyStratergy;
        Expression<Func<EquityPriceHistory, bool>> filter = e => e.Equity.IsActive
                                                                && e.Equity.GrowwRank >= GrowwRank;
        if (string.IsNullOrWhiteSpace(Code))
        {
            filter = filter.AndAlso(e => e.Equity.PE < 60 && e.Equity.ROE >= 25);

            if (buyStratergy == BuyStratergy.RSIBelow35)
                filter = filter.AndAlso(e => e.RSI <= 35);
            if (buyStratergy == BuyStratergy.RSI55To70)
                filter = filter.AndAlso(e => e.RSI <= 75 && e.RSI >= 65);
            if (buyStratergy == BuyStratergy.RSI14EMADiffLess1)
                filter = filter.AndAlso(e => e.RSI14EMADiff < -1);

            filter = filter.AndAlso(e => e.Date == Date);
        }
        else
        {
            filter = filter.AndAlso(e => e.Code.Contains(Code) || e.Equity.Name.Contains(Code));
        }
        var equityResult = HistoryRepo.FindAll(filter, includeProperties: "Equity,Equity.EquityStockCalculation", orderBy: e => e.OrderBy("Equity.GrowwRank", "DESC"));
        var trades = await equityResult.ResultObject.ToListAsync();
        TodaysTrades = Mapper.Map<List<EquityPriceHistoryDto>>(trades);

        var boughtStocks = await TradeRepo.FindAll(x => x.SellDate == null && x.Stratergy == buyStratergy).ResultObject.Select(x => x.Code).ToListAsync();
        TodaysTrades.ForEach(e => e.BuyAlready = boughtStocks.Contains(e.Code));
        IsLoading = false;
    }
    protected async Task BuyVirtualTrade(EquityPriceHistoryDto equity)
    {
        IsLoading = true;
        var tradeTaken = await TradeRepo.FindAll(x => x.DeletedOn == null && x.Stratergy == Stratergy && x.Code == equity.Code && !x.SellDate.HasValue).ResultObject.FirstOrDefaultAsync();
        if (tradeTaken is not null)
        {
            await NotificationService.Error($"Error : Trade is already available taken on {tradeTaken.BuyDate:dd-MMM-yyyy} with {tradeTaken.Stratergy}", "Error");
            IsLoading = false;
            equity.BuyAlready = true;
            return;
        }

        int quantity = 1 + (int)(15000 / equity.Close).ToFixed();

        var trade = new VirtualTrade
        {
            LTP         = equity.Close,
            Stratergy   = Stratergy,
            BuyDate     = Date,
            BuyRate     = equity.Close,
            Code        = equity.Code,
            Name        = equity.Name,
            Quantity    = quantity,
            BuyValue    = quantity * equity.Close,
            Target      = equity.Close + (equity.Close * 5 / 100),
            StopLoss    = equity.Close - (equity.Close * 7 / 100),
            EquityId    = equity.Equity.Id
        };

        var tradeResult = await TradeRepo.CreateAsync(trade, UserId);
        if (tradeResult.HasErrors)
        {
            await NotificationService.Error($"Error : {tradeResult.GetErrors()}", "Error");
            IsLoading = false;
            return;
        }
        await NotificationService.Success($"Success : Trade added to virual book successfully", "Success");
        IsLoading = false;
        equity.BuyAlready = true;
    }
    
    protected void BuyTrade(EquityPriceHistoryDto equity)
    {
        BuyStock = equity;
    }
}
