namespace ShareMarket.WebApp.Filters;

public class AppAuthorizeAttribute : TypeFilterAttribute
{
    public AppAuthorizeAttribute()
        : base(typeof(AuthorizationFilter)) =>
        Arguments = new[] { new AuthorizationRequirement() };

    public AppAuthorizeAttribute(UserRole role)
        : base(typeof(AuthorizationFilter)) =>
        Arguments = new[] { new AuthorizationRequirement(role) };

    public AppAuthorizeAttribute(UserRole[] roles)
       : base(typeof(AuthorizationFilter)) =>
       Arguments = new[] { new AuthorizationRequirement(roles) };

    public AppAuthorizeAttribute(UserRole[] roles, bool hasFamilyAccess)
      : base(typeof(AuthorizationFilter)) =>
      Arguments = new[] { new AuthorizationRequirement(roles, hasFamilyAccess) };

    public AppAuthorizeAttribute(bool hasFamilyAccess , bool hasFinanceAccess)
        : base(typeof(AuthorizationFilter)) =>
        Arguments = new[] { new AuthorizationRequirement(hasFamilyAccess, hasFinanceAccess) };
}
