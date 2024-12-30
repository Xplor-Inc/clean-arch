using Microsoft.AspNetCore.Authorization;

namespace ShareMarket.WebApp.Filters;

public class AuthorizationRequirement : IAuthorizationRequirement
{
    public UserRole?    Role                { get; set; }
    public UserRole[]?  Roles               { get; set; }
    public bool?        HasFamilyAccess     { get; set; }
    public bool?        HasFinanceAccess    { get; set; }
    
    public AuthorizationRequirement()
    {
    }
    public AuthorizationRequirement(UserRole userRole)
    {
        Role = userRole;
    }
    public AuthorizationRequirement(UserRole[] userRoles)
    {
        Roles = userRoles;
    }
    public AuthorizationRequirement(UserRole[] userRoles, bool hasFamilyAccess)
    {
        Roles           = userRoles;
        HasFamilyAccess = hasFamilyAccess;
    }
    public AuthorizationRequirement(bool hasFamilyAccess, bool hasFinanceAccess)
    {
        HasFamilyAccess  = hasFamilyAccess;
        HasFinanceAccess = hasFinanceAccess;
    }
}