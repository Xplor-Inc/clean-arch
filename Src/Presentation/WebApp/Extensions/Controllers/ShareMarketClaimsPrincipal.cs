namespace ShareMarket.WebApp.Extensions.Controllers;
public class ShareMarketClaimsPrincipal
{
    public virtual UserRole UserRole    { get; set; }
    public virtual long     UserId      { get; set; }
    public virtual long     MemberId    { get; set; }
}