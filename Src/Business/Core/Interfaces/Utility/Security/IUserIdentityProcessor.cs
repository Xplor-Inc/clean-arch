namespace ShareMarket.Core.Interfaces.Utility.Security;

public interface IUserIdentityProcessor
{
    Task<long> GetCurrentUserId();
}
