using ShareMarket.Core.Entities.Users;

namespace ShareMarket.Core.Interfaces.SqlServer;
public interface IDataContext<TUser> : IContext
        where TUser : User
{
    IQueryable<User>    Users   { get; }
}