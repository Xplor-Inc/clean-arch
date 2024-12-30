using ShareMarket.Core.Entities.Tradings;
using ShareMarket.Core.Hubs.SharekhanHub;
using ShareMarket.Core.Models.Dtos.Equities;
using ShareMarket.Core.Models.Dtos.Trading;
using ShareMarket.WebApp.Models.Pages.Tradebooks;


namespace ShareMarket.WebApp.Components.Pages.Tradings.TradeBooks;

public partial class TradeBookPage
{
    #region Parameters
    [Parameter]
    public string TabName { get; set; } = "portfolio";

    #endregion
 
    #region Properties

    private List<TradeBookDto>      FilterList                  { get; set; } = [];
    private List<TradeBookDto>      Tradings                    { get; set; } = [];
    private Validations             AddQuantityModelValidation  { get; set; } = new();
    private Validations             SellModelValidation         { get; set; } = new();
    private TradeOrderDto?          SellModel                   { get; set; }
    private TradeOrderDto?          TradeBookModel              { get; set; }
    private EquityStockDto?         EquityModel                 { get; set; }
    private TradeOrderDto?          AddQuantityModel            { get; set; }
    public TradeBookSummaryModel    SummaryModel                { get; set; } = default!;
    public TradeFilterModel         TradeFilter                 { get; set; } = new();
    private int                     DbUpdateCount               { get; set; }
    private bool                    Syncing                     { get; set; }
    private long                    ActiveTradeId               {  get; set; }
    #endregion
 
    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(TabName))
            TradeFilter.TabName = TabName;
        IsLoading = true;
        await GetDataAsync();
        await base.OnInitializedAsync();
    }

    protected async void RefreshByWebsocket(object? sender, SharekhanWebSocketEventArgs e)
    {
        DbUpdateCount++;
        foreach (var trade in Tradings)
        {
            var data = SharekhanWebSocket.Tradings.Find(x => x.ScripCode == trade.Equity.SharekhanId);
            if (data == null) continue;
            trade.MarketValue           = data.Ltp * trade.Quantity;
            trade.Equity.LTP            = data.Ltp;
            trade.Equity.DayChange      = data.RsChange;  
            trade.Equity.DayChangePer   = data.PerChange;
            trade.ReleasedPL            = trade.MarketValue - trade.BuyValue;
        }
        var holdings = Tradings.Where(x => x.SellDate is null);
        if (holdings.Any())
        {
            SummaryModel.Current            = Tradings.Sum(s => s.MarketValue - s.MarginAmount);
            SummaryModel.NonReleasedPL      = holdings.Sum(s => (s.Equity.LTP - s.BuyRate ?? 0) * s.Quantity);
            SummaryModel.NonReleasedPLPerc  = SummaryModel.NonReleasedPL / holdings.Sum(s => s.BuyRate == 0 ? 1 : s.BuyValue) * 100;
            SummaryModel.DayChange          = holdings.Sum(s => s.Equity.DayChange * s.Quantity);
            SummaryModel.DayChangePerc      = SummaryModel.DayChange / holdings.Sum(s => (s.Equity.LTP - s.Equity.DayChange) * s.Quantity) * 100;
        }
        if (DbUpdateCount > 500)
        {
            DbUpdateCount = 0;
            var ids = Tradings.Select(s => s.Id);
            var trades = await BookRepo.FindAll(e => ids.Contains(e.Id), includeProperties:nameof(TradeBook.Equity), asNoTracking : false).ResultObject.ToListAsync();
            foreach (var trade in trades)
            {
                var data = SharekhanWebSocket.Tradings.Find(x => x.ScripCode == trade.Equity.SharekhanId);
                if (data == null) continue;
                trade.MarketValue           = data.Ltp * trade.Quantity;
                trade.Equity.LTP            = data.Ltp;
                trade.Equity.LastSyncOn     = DateTime.Now.ToIst();
                trade.Equity.LTPDate        = DateOnly.FromDateTime(DateTimeOffset.Now.ToIst().Date);
                trade.Equity.DayChange      = data.RsChange;
                trade.Equity.DayChangePer   = data.PerChange;
                trade.ReleasedPL            = trade.MarketValue - trade.BuyValue;
            }

            var tradeUpdateResult = await BookRepo.UpdateAsync(trades, UserId);
            if (!tradeUpdateResult.HasErrors)
            {
                await NotificationService.Success("Trade book updated successfully by Websocket", "Success");
            }
        }
        Tradings = Tradings.OrderByDescending(o => o.CurrentStatus).ToList();
        await InvokeAsync(StateHasChanged);
    }
    
    protected async Task GetDataAsync()
    {
        ActiveTradeId = 0;
        Expression<Func<TradeBook, bool>> filter = e => e.DeletedOn == null && e.Orders.Any(x => x.BuyOrderId == null);
        if (TradeFilter.TabName == "portfolio")
        {
            filter = filter.AndAlso(e => e.Postion > 0);
        }
        if (TradeFilter.TabName == "report")
        {
            filter = filter.AndAlso(e => e.Postion == 0);
        }

        if (!string.IsNullOrEmpty(TradeFilter.Sector))
            filter = filter.AndAlso(e => e.Equity.Sector == TradeFilter.Sector);
        if (!string.IsNullOrEmpty(TradeFilter.Strategy))
            filter = filter.AndAlso(e => e.Strategy == TradeFilter.Strategy);
        if (!string.IsNullOrEmpty(TradeFilter.Account))
            filter = filter.AndAlso(e => e.TradingAccount == TradeFilter.Account);
        if (TradeFilter.TradeType != null)
            filter = filter.AndAlso(e => e.TradeType == TradeFilter.TradeType);

        var trades = await BookRepo.FindAll(filter, includeProperties: "Equity,Orders,Orders.Equity,Orders.SellOrders", 
                                                    orderBy: o => o.OrderBy("SellDate").ThenBy("BuyDate"))
                                             .ResultObject.ToListAsync();
        
        Tradings = Mapper.Map<List<TradeBookDto>>(trades);
        if (string.IsNullOrEmpty(TradeFilter.Sector) && TradeFilter.TradeType == null && string.IsNullOrEmpty(TradeFilter.Account))
            FilterList = Tradings;

        if (Tradings.Count > 0)
        {
            SummaryModel = new TradeBookSummaryModel
            {
                Invested = Tradings.Sum(s => s.BuyValue - s.MarginAmount)
            };

            var soldTrades = Tradings.Where(x => x.SellDate is not null);
            if (soldTrades.Any())
            {
                SummaryModel.ReleasedPL     = soldTrades.Sum(s => s.ReleasedPL);
                SummaryModel.ReleasedPLPerc = SummaryModel.ReleasedPL / soldTrades.Sum(s => s.BuyRate == 0 ? 1 : s.BuyValue) * 100;
            }
            var holdings = Tradings.Where(x => x.SellDate is null);
            if (holdings.Any())
            {
                SummaryModel.Current            = Tradings.Sum(s => s.MarketValue - s.MarginAmount);
                SummaryModel.NonReleasedPL      = holdings.Sum(s => (s.Equity.LTP - s.BuyRate??0) * s.Quantity);
                SummaryModel.NonReleasedPLPerc  = SummaryModel.NonReleasedPL / holdings.Sum(s => s.BuyRate == 0 ? 1 : s.BuyValue) * 100;
                SummaryModel.DayChange          = holdings.Sum(s => s.Equity.DayChange * s.Quantity);
                SummaryModel.DayChangePerc      = SummaryModel.DayChange / holdings.Sum(s => (s.Equity.LTP - s.Equity.DayChange) * s.Quantity) * 100;
            }
        }
        IsLoading = false;
        Tradings = [.. Tradings.OrderByDescending(o => o.CurrentStatus)];
    }

    protected async Task RefreshCurrentRates()
    {
        IsLoading = true;

        if (DateTime.Now.Hour < 16 && DateTime.Now.Hour > 8)
        {
            Syncing = true;
            var sharekhanSyncCodes = Tradings.Select(x => x.Equity.SharekhanId).ToList();
            await ConfigureWebSocket(sharekhanSyncCodes);
            SharekhanWebSocket.MessageReceived += RefreshByWebsocket;
            await NotificationService.Success("Web socket started successfully", "Success");
        }
        else
        {
            await TradeBookConductor.UpdateCurrentRatesAsync(CancellationToken.None);
            await GetDataAsync();
            await NotificationService.Success("Trade book updated successfully by Groww service", "Success");
        }
        IsLoading = false;
    }
  
    protected async Task AddQuantityAsync()
    {
        if (AddQuantityModel is null || !AddQuantityModel.TargetPerc.HasValue) return;
        var order = Mapper.Map<TradeOrder>(AddQuantityModel);
        if (order.OrderRate == 0)
        {
            string msg = $"You have entered buy rate as 0, Are you sure to procced?";
            var buyWithZero = await MessageService.Confirm(msg);
            if (!buyWithZero) { IsLoading = false; return; }
        }
        var existingTrade = await BookOrderRepo.FindAll(x => x.CreatedById  == UserId && x.EquityId == order.EquityId
                                                          && x.Strategy     == order.Strategy && x.TradeType == order.TradeType
                                                          && x.TradingAccount == order.TradingAccount && x.OrderStatus == OrderStatus.Open)
                                               .ResultObject.FirstOrDefaultAsync();
        if (existingTrade is not null)
        {
            string msg = $"You have an open trade for this stock with {existingTrade.Quantity} " +
                $"quantity bought on {existingTrade.OrderDate:dd-MMM-yyyy} at {existingTrade.OrderRate.ToCString()}";
            var buyMore = await MessageService.Confirm(msg);
            if (!buyMore) { IsLoading = false; return; }
        }
        order.OrderValue    = order.Quantity * order.OrderRate;
        if (order.TradeType != TradeType.Delivery)
            order.MarginAmount = order.GetMarginAmount;

        else
            order.MarginPerc = 100;

        order.OrderValue = (order.Quantity * order.OrderRate) - order.MarginAmount;

        var createBookResult = await TradeBookConductor.CreateTradeBookOrderAsync(order, UserId);
        if (createBookResult.HasErrors)
        {
            await NotificationService.Error(createBookResult.GetErrors(), "Error", x => x.Autohide = false);
            IsLoading = false;
            return;
        }
        else
            await NotificationService.Success($"{order.Quantity} added successfully for {order.Code}", "Success");

        IsLoading = false;
        AddQuantityModel = null;
        await GetDataAsync();
    }

    protected async Task SellTradeAsync()
    {
        if (SellModel is null || !SellModel.OrderRate.HasValue || !SellModel.OrderDate.HasValue || !SellModel.SellAction.HasValue) return;

        if (!await SellModelValidation.ValidateAll())
        {
            return;
        }
        IsLoading = true;

        var trade = await BookOrderRepo.FindAll(e => e.Id == SellModel.Id && e.CreatedById == UserId, 
                                                    includeProperties: "SellOrders", asNoTracking: false).ResultObject.FirstOrDefaultAsync();
        if (trade is null)
        {
            await NotificationService.Error("Trade not found");
            IsLoading = false;
            return;
        }
        int sold = 0;
        sold = trade.SellOrders?.Sum(o => o.Quantity) ?? 0;
        SellModel.AvailableToSell = trade.Quantity - sold;

        if (SellModel.AvailableToSell < SellModel.Quantity)
        {
            await NotificationService.Error($"Available trade quantity is {SellModel.AvailableToSell}, but trying to sell {SellModel.Quantity}");
            IsLoading = false;
            return;
        }

        var sellOrder = new TradeOrder
        {
            Code            = trade.Code,
            EquityId        = trade.EquityId,
            HoldingDays     = SellModel.OrderDate.Value.DayNumber - trade.OrderDate.DayNumber + 1,
            OrderDate       = SellModel.OrderDate.Value,
            BuyOrderId      = trade.Id,
            OrderStatus     = OrderStatus.Close,
            OrderType       = OrderType.Sell,
            TradeType       = trade.TradeType,
            TradingAccount  = trade.TradingAccount,
            Strategy        = trade.Strategy,
            SellAction      = SellModel.SellAction.Value,
            Remark          = SellModel.Remark,
            Quantity        = SellModel.Quantity,
            DailyMftInt     = trade.DailyMftInt,
            OrderRate       = SellModel.OrderRate.Value,
            OrderValue      = SellModel.Quantity * SellModel.OrderRate.Value,
            TradeBookId     = trade.TradeBookId,
        };
        
        var createResult = await BookOrderRepo.CreateAsync(sellOrder, UserId);
        if (createResult.HasErrors)
        {
            await NotificationService.Error(createResult.GetErrors());
            IsLoading = false;
            return;
        }
        await NotificationService.Success("Trade Traget update successfully");
        SellModel = null;
        IsLoading = false;
        await TradeBookConductor.AggregateTradeBookAsync(sellOrder.TradeBookId, UserId);
        await GetDataAsync();
    }
    
    protected void OpenSellPopup(TradeOrderDto trade)
    {
        SellModel = Mapper.Map<TradeOrderDto>(trade);
        SellModel.Quantity = SellModel.AvailableToSell;
        SellModel.OrderDate = null;
        SellModel.OrderRate = null;
    }

    protected void OpenAddMorePopup(TradeBookDto  trade)
    {
        AddQuantityModel = new TradeOrderDto
        {
            Code            = trade.Code,
            EquityId        = trade.EquityId,
            TradingAccount  = trade.TradingAccount,
            TradeBookId     = trade.Id,
            MarginPerc      = trade.MarginPerc,
            OrderType       = OrderType.Buy,
            OrderStatus     = OrderStatus.Open,
            Strategy        = trade.Strategy,
            TradeType       = trade.TradeType,
            TargetPerc      = trade.TargetPerc
        };
    }
    protected async Task DeleteTradeAsync(long Id)
    {
        IsLoading = true;
        var trade = await BookOrderRepo.FindAll(e => e.Id == Id && e.CreatedById == UserId, asNoTracking: false).ResultObject.FirstOrDefaultAsync();
        if (trade is null)
        {
            await NotificationService.Error("Trade not found");
            IsLoading = false;
            return;
        }
        string msg = $"Are you sure to delete trade of {trade.Code} with {trade.Quantity}";
        var delete = await MessageService.Confirm(msg);
        if (!delete) { IsLoading = false;  return; }

        var deleteResult = await BookOrderRepo.DeleteAsync(trade, UserId);
        if (deleteResult.HasErrors)
        {
            await NotificationService.Error(deleteResult.GetErrors());
            IsLoading = false;
            return;
        }
        await NotificationService.Success("Trade order deleted successfully");
        IsLoading = false;
        await GetDataAsync();
    }

    #region Filters

    protected async Task FilterByTab(string item)
    {
        TradeFilter.TabName = item;
        await GetDataAsync();
    }
  
    protected async Task FilterBySector(string? item)
    {
        TradeFilter.Sector = item;
        await GetDataAsync();
    }
    
    protected async Task FilterByStrategy(string? item)
    {
        TradeFilter.Strategy = item;
        await GetDataAsync();
    }
    
    protected async Task FilterByAccount(string? item)
    {
        TradeFilter.Account = item;
        await GetDataAsync();
    }
    
    protected async Task FilterByTradeType(TradeType? item)
    {
        TradeFilter.TradeType = item;
        await GetDataAsync();
    }

    protected void ShowHideOrders(long id)
    {
        if (ActiveTradeId == id)
            ActiveTradeId = 0;
        else
            ActiveTradeId = id;
    }
    #endregion

    protected async Task StopSyncing()
    {
        if (SharekhanWebSocket.IsConnected())
        {
            Syncing = false;
            await SharekhanWebSocket.DisConnect();
            await NotificationService.Success("Websocket stopped successfully");
        }
    }
    public async ValueTask DisposeAsync() => await StopSyncing();
}

public class TradeFilterModel
{
    public TradeType? TradeType { get; set; }
    public string? Account { get; set; }
    public string? Sector { get; set; }
    public string? Strategy { get; set; }
    public string TabName { get; set; } = "portfolio";

}