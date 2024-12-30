namespace ShareMarket.WebApp.Components.Accounts;

// This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
// every 30 minutes an interactive circuit is connected.
internal sealed class IdentityRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        return await ValidateSecurityStampAsync(authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(ClaimsPrincipal principal)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShareMarketContext>();
        var userId = principal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;
        if (userId == null)
        {
            return false;
        }

        var securityStamp = principal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.AuthenticationInstant)?.Value;
        var user = await dbContext.Users.FirstOrDefaultAsync(e => e.Id == long.Parse(userId));
        if (user == null || user.DeletedOn.HasValue || user.SecurityStamp != securityStamp)
        {
            return false;
        }

        return true;
    }
}
