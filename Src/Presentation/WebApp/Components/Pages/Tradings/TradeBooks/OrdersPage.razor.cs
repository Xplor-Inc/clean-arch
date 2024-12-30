using ShareMarket.Core.Entities.Tradings;
using ShareMarket.Core.Models.Dtos.Trading;

namespace ShareMarket.WebApp.Components.Pages.Tradings.TradeBooks;

public partial class OrdersPage
{
    #region Services
    [Inject] public required IRepositoryConductor<TradeOrder> BookOrderRepo { get; set; }

    #endregion

    public List<TradeOrderDto> Orders { get; set; } = [];
    public PagingDto PagingDto { get; set; } = new();
    public required SearchDto Search { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        await GetOrders(0);
        await base.OnInitializedAsync();
    }

    protected async Task GetOrders(int skip, string sortBy = "OrderDate", string sortOrder = "DESC")
    {
        IsLoading = true;
        Expression<Func<TradeOrder, bool>> filter = x => x.DeletedOn == null;
        if(Search.FromDate.HasValue)
            filter = filter.AndAlso(e=>e.OrderDate >= Search.FromDate.Value);
        if (Search.ToDate.HasValue)
            filter = filter.AndAlso(e => e.OrderDate <= Search.ToDate.Value);
        if (Search.OrderType.HasValue)
            filter = filter.AndAlso(e => e.OrderType == Search.OrderType);
        if (!string.IsNullOrEmpty(Search.ScripCode))
            filter = filter.AndAlso(e => e.Code == Search.ScripCode || e.Equity.Name.StartsWith(Search.ScripCode));

        var orders = await BookOrderRepo.FindAll(filter,
                                                      orderBy: e => e.OrderBy(sortBy, sortOrder).ThenBy("CreatedOn"),
                                                      includeProperties: "SellOrders,Equity", take: PagingDto.ItemPerPage, skip: skip)
                                        .ResultObject.ToListAsync();

        PagingDto.RowCount  = await BookOrderRepo.FindAll(filter).ResultObject.LongCountAsync();
        PagingDto.Skip      = skip;
        PagingDto.SortOrder = sortOrder;
        PagingDto.SortBy    = sortBy;
        Orders = Mapper.Map<List<TradeOrderDto>>(orders);
        IsLoading = false;
        StateHasChanged();
    }

    protected async Task NextPage()
    {
        PagingDto.Skip += PagingDto.ItemPerPage;
        await GetOrders(PagingDto.Skip, PagingDto.SortBy, PagingDto.SortOrder);
    }

    protected async void PrevPage()
    {
        PagingDto.Skip -= PagingDto.ItemPerPage;
        await GetOrders(PagingDto.Skip, PagingDto.SortBy, PagingDto.SortOrder);
    }
   
    private async Task SortData(PagingDto options)
    {
        //string sortOrder = PagingDto.SortOrder == "DESC" ? "ASC" : "DESC";
        //sortOrder = PagingDto.SortBy == sortBy ? sortOrder : "ASC";
        await GetOrders(0, options.SortBy, options.SortOrder);
    }

}
public class SearchDto
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public OrderType? OrderType { get; set; }
    public string? ScripCode { get; set; }
}

public class PagingDto
{
    public int      Skip        { get; set; }
    public long     RowCount    { get; set; }
    public int      ItemPerPage { get; set; } = 20;
    public string   SortOrder   { get; set; } = "DESC";
    public string   SortBy      { get; set; } = "OrderDate";
}