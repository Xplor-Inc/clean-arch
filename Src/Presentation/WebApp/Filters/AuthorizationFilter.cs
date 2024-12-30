using Microsoft.AspNetCore.Mvc.Filters;

namespace ShareMarket.WebApp.Filters;

public class AuthorizationFilter(AuthorizationRequirement requirement) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context is null || context.HttpContext is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // If the user is not authenticated, return a 401
        if (!context.HttpContext.User?.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var claim = context.HttpContext.User?.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Role)?.Value;
        if (claim is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var claimRole = Enum.Parse<UserRole>(claim);
        if (claimRole != UserRole.SuperAdmin)
        {
            if (requirement.Role.HasValue && claimRole != requirement.Role)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (requirement.Roles != null && !requirement.Roles.Contains(claimRole))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        //if (Requirement.HasFamilyAccess.HasValue && Requirement.HasFamilyAccess.Value)
        //{
        //    var hasFamilyAccess = context.HttpContext.User?.Claims.FirstOrDefault(e => e.Type == ClaimTypeConstant.HasFamilyAccess)?.Value;
        //    if (!string.Equals(hasFamilyAccess, $"{Requirement.HasFamilyAccess.Value}"))
        //    {
        //        context.Result = new UnauthorizedResult();
        //        return;
        //    }
        //}

        //if (Requirement.HasFinanceAccess.HasValue && Requirement.HasFinanceAccess.Value)
        //{
        //    var hasFinanceAccess = context.HttpContext.User?.Claims.FirstOrDefault(e => e.Type == ClaimTypeConstant.HasFinanceAccess)?.Value;
        //    if (!string.Equals(hasFinanceAccess, $"{Requirement.HasFinanceAccess.Value}"))
        //    {
        //        context.Result = new UnauthorizedResult();
        //        return;
        //    }
        //}
    }
}