namespace ShareMarket.WebApp.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static UserRole? RoleType(this ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        if (principal.IsUnauthenticated())
        {
            return null;
        }

        var roleIdClaim = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleIdClaim == null)
        {
            return null;
        }

        return Enum.Parse<UserRole>(roleIdClaim.Value);
    }
  
    public static bool IsAuthenticated(this ClaimsPrincipal principal) => principal?.Identity?.IsAuthenticated ?? false;

    public static bool IsUnauthenticated(this ClaimsPrincipal principal) => !principal.IsAuthenticated();

    public static long UserId(this ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }
        var userIdClaim = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
        if (userIdClaim == null)
        {
            return 0;
        }
        return long.Parse(userIdClaim.Value);
    }
    //public static long MemberId(this ClaimsPrincipal principal)
    //{
    //    if (principal == null)
    //    {
    //        throw new ArgumentNullException(nameof(principal));
    //    }
    //    var userIdClaim = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypeConstant.MemberId);
    //    if (userIdClaim == null)
    //    {
    //        return 0;
    //    }
    //    return long.Parse(userIdClaim.Value);
    //}
    //public static bool HasFinanceAccess(this ClaimsPrincipal principal)
    //{
    //    if (principal == null)
    //    {
    //        throw new ArgumentNullException(nameof(principal));
    //    }
    //    var userIdClaim = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypeConstant.HasFinanceAccess);
    //    if (userIdClaim == null)
    //    {
    //        return false;
    //    }
    //    return bool.Parse(userIdClaim.Value);
    //}
    //public static bool HasFamilyAccess(this ClaimsPrincipal principal)
    //{
    //    if (principal == null)
    //    {
    //        throw new ArgumentNullException(nameof(principal));
    //    }
    //    var userIdClaim = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypeConstant.HasFamilyAccess);
    //    if (userIdClaim == null)
    //    {
    //        return false;
    //    }
    //    return bool.Parse(userIdClaim.Value);
    //}
}
