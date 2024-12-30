using Microsoft.AspNetCore.Mvc.Filters;

namespace ShareMarket.WebApp.Extensions.Controllers;
public abstract class ShareMarketController : ControllerController
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
    }

    protected virtual ShareMarketClaimsPrincipal XploringMeClaims { get; set; } = default!;
    protected virtual UserRole? CurrentRoleType => XploringMeClaims != null ? XploringMeClaims.UserRole : User.RoleType();
    protected virtual long CurrentUserId => XploringMeClaims != null ? XploringMeClaims.UserId : User.UserId();
    //protected virtual long MemberId => User.MemberId();
    //protected virtual bool HasFamilyAccess => User.HasFamilyAccess();
    //protected virtual bool HasFinanceAccess => User.HasFinanceAccess();
    protected const int Take = 100;

}