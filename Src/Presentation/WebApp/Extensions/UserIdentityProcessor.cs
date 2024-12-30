using ShareMarket.Core.Interfaces.Utility.Security;

namespace ShareMarket.WebApp.Extensions;

public class UserIdentityProcessor(AuthenticationStateProvider AuthenticationStateAsync) : IUserIdentityProcessor
{
    public async Task<long> GetCurrentUserId()
    {
        var authstate = await AuthenticationStateAsync.GetAuthenticationStateAsync() ?? throw new InvalidOperationException("User is not authorized");
        var userIdClaim = authstate.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Sid) ?? throw new InvalidOperationException("User is not authorized");
        return long.Parse(userIdClaim.Value);
    }
}
