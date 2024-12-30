using Blazorise.Components;
using ShareMarket.Core.Entities.Tradings;
using ShareMarket.Core.Models.Dtos.Trading;

namespace ShareMarket.WebApp.Components.Pages.Tradings.TradeBooks;

public partial class TradeBookOrderForm
{
    #region Parameters
    [Parameter]
    public string               BuyStrategy { get; set; } = default!;
    [Parameter]
    public string               StockCode   { get; set; } = default!;
    [Parameter]
    public long                 EquityId    { get; set; }
    [Parameter]
    public decimal?             BuyRate     { get; set; }
    [Parameter]
    public decimal              TargetPerc  { get; set; }
    [Parameter]
    public EventCallback<bool>  CloseForm   { get; set; }
    [Parameter]
    public EventCallback<bool>  OnClose     { get; set; }
    #endregion

    #region Properties
    protected TradeOrderDto?        TradeBookModel              { get; set; } = default!;
    protected List<string>          Statergies                  { get; set; } = ["RSI Below 35", "RSI 55-70"];
    protected List<string>          Accounts                    { get; set; } = ["Account-1", "Account-2"];
    protected Validations           TradeBookModelValidation    { get; set; } = new();
    protected List<SelectListItem>  AutoCompleteCodes           { get; set; } = [];
    #endregion

    protected override Task OnInitializedAsync()
    {
        TradeBookModel = new TradeOrderDto { Strategy = BuyStrategy, Code = StockCode, EquityId = EquityId, OrderRate = BuyRate, TargetPerc = TargetPerc };
        return base.OnInitializedAsync();
    }

    protected async Task Buy()
    {
        if (!await TradeBookModelValidation.ValidateAll())
        {
            return;
        }
        IsLoading = true;
        var order = Mapper.Map<TradeOrder>(TradeBookModel);
        if(order.OrderRate == 0)
        {
            string msg = $"You have entered buy rate as 0, Are you sure to procced?";
            var buyWithZero = await MessageService.Confirm(msg);
            if (!buyWithZero) { IsLoading = false; return; }
        }
        if (order.EquityId == 0)
        {
            var equityStock = await EquityRepo.FindAll(x => x.Code == order.Code).ResultObject.FirstOrDefaultAsync();
            if(equityStock is null)
            {
                await NotificationService.Error($"Equity Stock not found with code {order.Code}", "Error");
                IsLoading = false;
                return;
            }
            order.EquityId = equityStock.Id;
        }
        var existingTrade = await BookOrderRepo.FindAll(x=> x.CreatedById == UserId && x.EquityId == order.EquityId && x.DeletedOn == null
                                                            && x.Strategy == order.Strategy && x.TradeType == order.TradeType
                                                         && x.TradingAccount == order.TradingAccount && x.OrderStatus == OrderStatus.Open)
                             .ResultObject.FirstOrDefaultAsync();
        if (existingTrade != null)
        {
            string msg = $"You have an open trade for this stock with {existingTrade.Quantity} " +
                $"quantity bought on {existingTrade.OrderDate:dd-MMM-yyyy} at {existingTrade.OrderRate.ToCString()}";
            var buyMore = await MessageService.Confirm(msg);
            if(!buyMore) { IsLoading = false;   return; }
        }
        order.OrderStatus = OrderStatus.Open;
        order.OrderType = OrderType.Buy;
        order.OrderValue = order.Quantity * order.OrderRate;
        if (order.TradeType != TradeType.Delivery)
        {
            order.MarginAmount = order.GetMarginAmount;
        }
        else
        {
            order.MarginPerc = 100;
        }
        order.OrderValue = (order.Quantity * order.OrderRate) - order.MarginAmount;

        var createBookResult = await TradeBookConductor.CreateTradeBookOrderAsync(order, UserId);
        if (createBookResult.HasErrors)
            await NotificationService.Error(createBookResult.GetErrors(), "Error", x => x.Autohide = false);

        else
            await NotificationService.Success("Trade book created successfully", "Success");

        IsLoading = false;
        await CloseForm.InvokeAsync();
        if (OnClose.HasDelegate)
            await OnClose.InvokeAsync();        
    }

    protected async Task OnHandleReadData(AutocompleteReadDataEventArgs args)
    {
        if (args.CancellationToken.IsCancellationRequested) return;

        string search = args.SearchValue;
        AutoCompleteCodes = await EquityRepo.FindAll(x => x.Code.StartsWith(search) || x.Name.StartsWith(search))
                                            .ResultObject
                                            .Select(x => new SelectListItem { Key = x.Code, Value = $"{x.Code}-{x.Name}" })
                                            .ToListAsync(args.CancellationToken);
    }
}
