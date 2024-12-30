using Hangfire.Storage;
using Newtonsoft.Json;
using ShareMarket.WebApp.Models;

namespace ShareMarket.WebApp.Components.Pages.Tradings.FnO;

public partial class FnOTrading
{
    private FilterModel Filters { get; set; } = new();
    public List<OrderList> Tradings { get; set; } = [];
    private List<string> Indexes { get; set; } = [];
    private List<string> References { get; set; } = [];
    private List<DateTime> Days { get; set; } = [];

    private bool IsLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var x = await File.ReadAllTextAsync("C:\\Users\\hoshi\\OneDrive\\Desktop\\optionOrders.json") ;

        var rowData = JsonConvert.DeserializeObject<List<OrderList>>(x) ?? [];

        Tradings = [.. rowData.OrderByDescending(x => x.CreatedAt)];
        Days = rowData.Select(x => x.TradeDate).Distinct().ToList();
        Indexes = rowData.Select(x => x.SymbolName.Split(' ')[0]).Distinct().ToList();



        await base.OnInitializedAsync();
    }

    protected async Task LoadData(string? reference, string? index, TradeType? contractType)
    {
        IsLoading = true;
        Filters.ActiveRef = reference;
        Filters.ActiveIndex = index;
        Filters.ContractType = contractType; 
        var x = await File.ReadAllTextAsync("C:\\Users\\hoshi\\OneDrive\\Desktop\\optionOrders.json");

        var rowData = JsonConvert.DeserializeObject<List<OrderList>>(x) ?? [];

        Tradings = [.. rowData.OrderByDescending(x => x.CreatedAt)];
        Tradings = Tradings.FindAll(e => e.SymbolName.Split(' ')[0] == Filters.ActiveIndex || Filters.ActiveIndex == null);

        Days = Tradings.Select(x => x.TradeDate).Distinct().ToList();
        //&& (e.Ref == Filters.ActiveRef || Filters.ActiveRef == null)
        //&& (e.ContractType == contractType || contractType == null)).ResultObject.ToListAsync();
        //Tradings = Mapper.Map<List<OptionTradingDto>>(data);
        //var months = CommonUtils.GetMonths(Tradings.Min(x => x.BuyDate), Tradings.Max(x => x.BuyDate));
        //MonthWiseTrades = [];
        //foreach (var item in months)
        //{
        //    var trades = Tradings.Where(x => x.BuyDate?.Month == item.Month && x.BuyDate?.Year == item.Year);
        //    MonthWiseTrades.Add(new OptionTradingDto
        //    {
        //        StrickPrice = trades.Count(),
        //        BuyDate = item,
        //        Brokerage = trades.Sum(s => s.Brokerage),
        //        NetPL = trades.Sum(s => s.NetPL),
        //        PL = trades.Max(s => s.GetBuyAmount()),
        //        Index = $"{trades.Count(x => x.NetPL > 0)}/{trades.Count()}"
        //    });
        //}

        //Days = data.Select(s => s.BuyDate).Distinct().ToList();
        //if (Filters.ActiveDay.Year == 1 && !Tradings.Any(x => x.Analysis is null))
        //    Filters.ActiveDay = Days.FirstOrDefault();
        //IsLoading = false;
        StateHasChanged();
    }

    protected void ToggleAccordian(DateOnly date)
    {
        Filters.ActiveDay = Filters.ActiveDay == date ? DateOnly.MinValue : date;
    }

}


public class FilterModel
{
    public DateOnly ActiveDay { get; set; }
    public string? ActiveRef { get; set; }
    public string? ActiveIndex { get; set; }
    public TradeType? ContractType { get; set; }
}
