using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Models.Dtos.Equities;

namespace ShareMarket.WebApp.Components.Pages.Tradings.TradeBooks;

public partial class EquityForm
{
    #region Parameters
    [Parameter]
    public EventCallback<bool> CloseForm    { get; set; }
    [Parameter]
    public EventCallback<bool> OnClose      { get; set; }
    #endregion

    #region Properties
    public EquityStockDto?  EquityModel             { get; set; } = new();
    private Validations     EquityModelValidation   { get; set; } = new();
    #endregion

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    protected async Task CreateEntity()
    {
        if (!await EquityModelValidation.ValidateAll()) return;

        IsLoading = true;
        var equity = Mapper.Map<EquityStock>(EquityModel);

        var existingTrade = await EquityRepo.FindAll(x => x.Code == equity.Code && x.DeletedOn == null)
                             .ResultObject.FirstOrDefaultAsync();
        if (existingTrade != null)
        {
            string msg = $"Equity already exist with code {existingTrade.Code}";
            await NotificationService.Error(msg, "Error", x => x.Autohide = false);
            return;
        }
        var x = await GrowwService.GetLTPPrice(equity.Code);
        if (x.HasErrors || x.ResultObject == null || x.ResultObject.Ltp == 0)
        {
            string msg = $"Equity/EFT price data not found with code {equity.Code} on groww";
            await NotificationService.Error(msg, "Error", x => x.Autohide = false);
            return;
        }
        equity.EquityPanditUrl = equity.GetEquityPanditUrl;

        equity.BSECode = equity.Code;
        var createBookResult = await EquityRepo.CreateAsync(equity, UserId);
        if (createBookResult.HasErrors)
            await NotificationService.Error(createBookResult.GetErrors(), "Error", x => x.Autohide = false);

        else
            await NotificationService.Success("Equity created successfully", "Success");

        IsLoading = false;
        await CloseForm.InvokeAsync();
        if (OnClose.HasDelegate)
            await OnClose.InvokeAsync();
    }

   
}
