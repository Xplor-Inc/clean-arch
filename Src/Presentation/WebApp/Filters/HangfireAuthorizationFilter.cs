using Hangfire.Dashboard;

namespace ShareMarket.WebApp.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;// context.GetHttpContext()?.User?.Identity?.IsAuthenticated ?? false;
    }
}
