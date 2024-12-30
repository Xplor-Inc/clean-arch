namespace ShareMarket.WebApp.Utilities;
public class AuthenticationUtils
{
    public static AuthenticationProperties GetAuthenticationProperties()
    {
        return new AuthenticationProperties
        {
            AllowRefresh    = true,
            IssuedUtc       = DateTimeOffset.Now.ToIst(),
            ExpiresUtc      = DateTimeOffset.Now.ToIst().AddHours(24),
            IsPersistent    = true
        };
    }
}