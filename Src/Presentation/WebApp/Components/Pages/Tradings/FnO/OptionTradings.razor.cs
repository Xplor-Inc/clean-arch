using Newtonsoft.Json;
using ShareMarket.Core.Utilities;
using ShareMarket.WebApp.Models;

namespace ShareMarket.WebApp.Components.Pages.Tradings.FnO;
public class FilterModel
{
    public DateOnly ActiveDay { get; set; }
    public string? ActiveRef { get; set; }
    public string? ActiveIndex { get; set; }
    public TradeType? ContractType { get; set; }
}
public partial class OptionTradings
{
    private bool                        IsLoading               { get; set; } = true;
    private List<string>                Indexes                 { get; set; } = [];
    private List<string>                References              { get; set; } = [];
    private List<OptionTradingDto>      Tradings                { get; set; } = [];
    private List<OptionTradingDto>      MonthWiseTrades         { get; set; } = [];
    private Validations                 OTModelValidation       { get; set; } = new();
    private Validations                 SellModelValidation     { get; set; } = new();
    private Validations                 AddLotModelValidation   { get; set; } = new();
    private Validations                 AnalysisModelValidation { get; set; } = new();
    private OptionTradingDto?           TradeModel              { get; set; }
    private List<DateOnly>              Days                    { get; set; } = [];
    private FilterModel                 Filters                 { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadData(Filters.ActiveRef, Filters.ActiveIndex, Filters.ContractType);
        IsLoading = false;

        var x = await File.ReadAllTextAsync("C:\\Users\\hoshi\\OneDrive\\Desktop\\optionOrders.json");
        var rowData = JsonConvert.DeserializeObject<List<OrderList>>(x) ?? [];
        //References = rowData.Select(s => s.Ref).Distinct().ToList();
        References.Sort();
        Indexes = rowData.Select(x => x.SymbolName.Split(' ')[0]).Distinct().ToList();
        Indexes.Sort();
    }

    protected async Task LoadData(string? reference, string? index, TradeType? contractType)
    {
        IsLoading = true;
        Filters.ActiveRef = reference;
        Filters.ActiveIndex = index;
        Filters.ContractType = contractType;

        var x = await File.ReadAllTextAsync("C:\\Users\\hoshi\\OneDrive\\Desktop\\optionOrders.json");
        var rowData = JsonConvert.DeserializeObject<List<OrderList>>(x) ?? [];
        rowData = rowData.FindAll(x => x.TradeDate.Month > 3);
        rowData = [.. rowData.OrderBy(x => x.CreatedAt)];
        //var data = rowData.FindAll(e => (e.Index == Filters.ActiveIndex || Filters.ActiveIndex == null)
        //                                        && (e.Ref == Filters.ActiveRef || Filters.ActiveRef == null)
        //                                        && (e.ContractType == contractType || contractType == null));

        foreach (var item in rowData)
        {
            var dsp = item.DisplayName.Split(' ');
            if (item.BuySell == "S")
            {
                bool update = true;
                foreach (var buy in Tradings.Where(x => x.ContractId == item.ContractId && x.OrderStatus == OrderStatus.Open))
                {
                    
                    // buy.Analysis = "item.Analysis";
                    buy.OrderStatus = OrderStatus.Close;
                    buy.SellDate    = DateOnly.FromDateTime(item.TradeDate);
                    buy.BuyDate     = DateOnly.FromDateTime(item.TradeDate);
                    buy.SellRate    = item.AvgFillPrice / 100;
                    buy.PL          = buy.GetPLAmount();

                    var orderValue1 = buy.GetSellAmount();
                    if (buy.ContractType == TradeType.Future)
                        orderValue1 = 85000;
                    if (update)
                    {
                        buy.Brokerage += BrokarageCharges.Groww(orderValue1, OrderType.Sell, TradeType.Options);
                        update = false;
                    }
                    buy.NetPL = buy.PL - buy.Brokerage;
                }
                continue;
            }
            var order = new OptionTradingDto
            {
                BuyDate         = DateOnly.FromDateTime(item.TradeDate),
                BuyRate         = item.AvgFillPrice / 100,
                ContractType    = TradeType.Options,
                ExpireDate      = DateOnly.Parse($"{dsp[1]} {dsp[2]} 2024"),
                Index           = dsp[0],
                OptionType      = OptionType.Call,
                Quantity        = item.Qty,
                SellDate        = DateOnly.FromDateTime(item.TradeDate),
                OrderStatus     = OrderStatus.Open,
                ContractId      = item.ContractId
            };

            if (item.DisplayName.EndsWith(" Fut"))
                order.ContractType  = TradeType.Future;
            if (item.DisplayName.EndsWith(" Put"))
                order.OptionType    = OptionType.Put;
            if (order.ContractType == TradeType.Future)
                order.StrickPrice   = item.Price;
            if (order.ContractType == TradeType.Options)
                order.StrickPrice   = Convert.ToInt32(dsp[3])/100;

            var orderValue = order.GetBuyAmount();
            if (order.ContractType == TradeType.Future)
                orderValue = 85000;
            if (item.BuySell == "B")
                order.Brokerage = BrokarageCharges.Groww(orderValue, OrderType.Buy, TradeType.Options);
            else
                order.Brokerage = BrokarageCharges.Groww(orderValue, OrderType.Sell, TradeType.Options);
            
            Tradings.Add(order);
        }

        Tradings = Tradings.FindAll(e => (e.Index == Filters.ActiveIndex || Filters.ActiveIndex == null)
                                                && (e.Ref == Filters.ActiveRef || Filters.ActiveRef == null)
                                                && (e.ContractType == contractType || contractType == null));

        var months = CommonUtils.GetMonths(Tradings.Min(x => x.BuyDate), Tradings.Max(x => x.BuyDate));
        MonthWiseTrades = [];
        foreach (var item in months)
        {
            var trades = Tradings.Where(x => x.BuyDate.Month == item.Month && x.BuyDate.Year == item.Year);

            MonthWiseTrades.Add(new OptionTradingDto
            {
                StrickPrice = trades.Count(),
                BuyDate = item,
                Brokerage = trades.Sum(s => s.Brokerage),
                NetPL = trades.Sum(s => s.NetPL),
                PL = trades.Max(s => s.GetBuyAmount()),
                Index = $"{trades.Count(x => x.NetPL > 0)}/{trades.Count()}"
            });
        }

        Days = Tradings.Select(s => s.BuyDate).Distinct().ToList();
        if (Filters.ActiveDay.Year == 1 && !Tradings.Any(x => x.Analysis is null))
            Filters.ActiveDay = Days.FirstOrDefault();
        IsLoading = false;
    }

    protected async Task BuyTrade()
    {
        //if(TradeModel == null) return;
        //if (!await OTModelValidation.ValidateAll())
        //{
        //    return;
        //}
        //IsLoading = true;
        //TradeModel.OpenPosition = TradeModel.Quantity ?? 0;
        //TradeModel.Brokerage = BrokarageCharges.Groww(TradeModel.GetBuyAmount(), TradeAction.Buy, TradeType.Options);
        //if (TradeModel.SellRate > 0)
        //{
        //    var sellCharges = BrokarageCharges.Groww(TradeModel.GetSellAmount(), TradeAction.Sell, TradeType.Options);

        //    TradeModel.Brokerage       += sellCharges;
        //    TradeModel.PL              = TradeModel.GetPLAmount();
        //    TradeModel.NetPL           = TradeModel.PL - TradeModel.Brokerage;
        //    TradeModel.OpenPosition    = 0;
        //}
        //var trade = Mapper.Map<OptionTrading>(TradeModel);
        //var updateResult = TradingRepo.Create(trade, UserId);
        //if (updateResult.HasErrors)
        //{
        //    await NotificationService.Error(updateResult.GetErrors());
        //    IsLoading = false;
        //    return;
        //}
        //await NotificationService.Success("Trade added successfully");
        //IsLoading = false;
        //if (!References.Contains(TradeModel.Ref))
        //{
        //    References.Add(TradeModel.Ref);
        //    References.Sort();
        //}
        //if (!Indexes.Contains(TradeModel.Index))
        //{
        //    Indexes.Add(TradeModel.Index);
        //    Indexes.Sort();
        //}
        //TradeModel = null;
        //await LoadData(Filters.ActiveRef, Filters.ActiveIndex, Filters.ContractType);
        //await NotificationService.Success("Data refreshed successfully");
    }

    //protected async Task SellTrade()
    //{
    //    if (SellModel is null) return;

    //    if (!await SellModelValidation.ValidateAll())
    //    {
    //        return;
    //    }
    //    IsLoading = true;

    //    var trade = await TradingRepo.FindAll(e => e.Id == SellModel.Id && e.CreatedById == UserId).ResultObject.FirstOrDefaultAsync();
    //    if (trade is null)
    //    {
    //        await NotificationService.Error("Trade not found");
    //        IsLoading = false;
    //        return;
    //    }

    //    var charges = BrokarageCharges.Groww(SellModel.GetSellAmount(), TradeAction.Sell, TradeType.Options);
    //    trade.Brokerage     += charges;
    //    trade.SellDate      = SellModel.SellDate;
    //    trade.SellRate      = GetAverageRate(trade.SellRate, (trade.Quantity - trade.OpenPosition), SellModel.SellRate, SellModel.Quantity);
    //    trade.SellAction    = SellModel.SellAction;
    //    trade.HasSL         = SellModel.HasSL;
    //    trade.OpenPosition  -= SellModel.Quantity ?? 0;
    //    trade.PL            = trade.GetPLAmount();
    //    trade.NetPL         = trade.PL - trade.Brokerage;
    //    var updateResult = TradingRepo.Update(trade, UserId);
    //    if (updateResult.HasErrors)
    //    {
    //        await NotificationService.Error(updateResult.GetErrors());
    //        IsLoading = false;
    //        return;
    //    }
    //    await NotificationService.Success("Trade sell successfully");
    //    SellModel = null;
    //    IsLoading = false;
    //    await LoadData(Filters.ActiveRef, Filters.ActiveIndex, Filters.ContractType);
    //}

    //protected async Task UpdateTradeAnalysis()
    //{
    //    if (AnalysisModel is null) return;

    //    if (!await AnalysisModelValidation.ValidateAll())
    //    {
    //        return;
    //    }
    //    IsLoading = true;

    //    var trade = await TradingRepo.FindAll(e => e.Id == AnalysisModel.Id && e.CreatedById == UserId).ResultObject.FirstOrDefaultAsync();
    //    if (trade is null)
    //    {
    //        await NotificationService.Error("Trade not found");
    //        IsLoading = false;
    //        return;
    //    }
    //    trade.Analysis = AnalysisModel.Analysis;
    //    var updateResult = TradingRepo.Update(trade, UserId);
    //    if (updateResult.HasErrors)
    //    {
    //        await NotificationService.Error(updateResult.GetErrors());
    //        IsLoading = false;
    //        return;
    //    }
    //    await NotificationService.Success("Trade Analysis update successfully");
    //    AnalysisModel = null;
    //    IsLoading = false;
    //    await LoadData(Filters.ActiveRef, Filters.ActiveIndex, Filters.ContractType);
    //}

    //protected async Task DeleteTrade(OptionTradingDto trade)
    //{
    //    if (!await MessageService.Confirm($"Are you sure, you want to delete  {trade.Index} {trade.ExpireDate: dd MMM} {trade.StrickPrice} {trade.OptionType} Contract?", "Delete Contract"))
    //    {
    //        return;
    //    }
    //    IsLoading = true;
    //    var tradeModel = await TradingRepo.FindAll(e => e.Id == trade.Id && e.CreatedById == UserId).ResultObject.FirstOrDefaultAsync();
    //    if (tradeModel is null)
    //    {
    //        await NotificationService.Error("Trade not found");
    //        IsLoading = false;
    //        return;
    //    }
    //    var deleteResult = TradingRepo.Delete(tradeModel, UserId);
    //    if (deleteResult.HasErrors)
    //    {
    //        await NotificationService.Error(deleteResult.GetErrors());
    //        IsLoading = false;
    //        return;
    //    }
    //    await NotificationService.Success("Trade deleted successfully");
    //    await LoadData(Filters.ActiveRef, Filters.ActiveIndex, Filters.ContractType);
    //    IsLoading = false;
    //}

    //private static decimal GetAverageRate(decimal rate1, int quantity1, decimal? rate2, int? quantity2)
    //{
    //    if (rate1 == 0) return rate2 ?? 0;
    //    if (rate2 == 0) return rate1;
    //    var a1 = rate1 * quantity1;
    //    var a2 = (rate2 * quantity2) ?? 0;
    //    var avg = (a1 + a2) / (quantity1 + quantity2 ?? 0);
    //    return avg.ToFixed();
    //}

    //protected async Task AddNewLot()
    //{
    //    if (AddLotModel is null) return;
    //    if (!await AddLotModelValidation.ValidateAll())
    //    {
    //        return;
    //    }

    //    IsLoading = true;

    //    var trade = await TradingRepo.FindAll(e => e.Id == AddLotModel.Id && e.CreatedById == UserId).ResultObject.FirstOrDefaultAsync();
    //    if (trade is null)
    //    {
    //        await NotificationService.Error("Trade not found");
    //        IsLoading = false;
    //        return;
    //    }
    //    var charges = BrokarageCharges.Groww(AddLotModel.GetBuyAmount(), TradeAction.Buy, TradeType.Options);
    //    trade.Brokerage     += charges;
    //    trade.BuyRate       = GetAverageRate(trade.BuyRate, trade.Quantity, AddLotModel.BuyRate, AddLotModel.Quantity);
    //    trade.OpenPosition  += AddLotModel.Quantity ?? 0;
    //    trade.Quantity      += AddLotModel.Quantity ?? 0;
    //    var updateResult = TradingRepo.Update(trade, UserId);
    //    if (updateResult.HasErrors)
    //    {
    //        await NotificationService.Error(updateResult.GetErrors());
    //        IsLoading = false;
    //        return;
    //    }
    //    await NotificationService.Success("New lot added successfully");
    //    AddLotModel = null;
    //    IsLoading = false;
    //    await LoadData(Filters.ActiveRef, Filters.ActiveIndex, Filters.ContractType);
    //}

    protected string RowColor(OptionTradingDto row)
    {
        if (row.NetPL > 0) return "#008000";
        if (row.NetPL <= 0) return "#FF0000";
        return string.Empty;
    }

    //protected void OpenSellWindow(OptionTradingDto dto)
    //{
    //    SellModel = new OptionTradingSellDto
    //    {
    //        Id          = dto.Id,
    //        BuyDate     = dto.BuyDate,
    //        ExpireDate  = dto.ExpireDate,
    //        Heading     = $"{dto.Index} {dto.ExpireDate: dd MMM} {dto.StrickPrice} {dto.OptionType}",
    //    };
    //}

    //protected void OpenAddWindow(OptionTradingDto dto)
    //{
    //    AddLotModel = new OptionTradingAddLotDto
    //    {
    //        Id      = dto.Id,
    //        Heading = $"{dto.Index} {dto.ExpireDate: dd MMM} {dto.StrickPrice} {dto.OptionType}",
    //    };
    //}

    //protected void OpenAnalysisWindow(OptionTradingDto dto)
    //{
    //    AnalysisModel = new OptionTradingAnalysisDto
    //    {
    //        Id      = dto.Id,
    //        Heading = $"{dto.Index} {dto.ExpireDate: dd MMM} {dto.StrickPrice} {dto.OptionType}",
    //    };
    //}

    protected void ToggleAccordian(DateOnly date)
    {
        Filters.ActiveDay = Filters.ActiveDay == date ? DateOnly.MinValue : date;
    }
}
