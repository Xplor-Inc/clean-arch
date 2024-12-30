using ShareMarket.Core.Entities.Audits;

namespace ShareMarket.Core.Interfaces.SqlServer;
public interface IShareMarketContext : IContext
{
    IQueryable<ChangeLog>       ChangeLogs          { get; }
    IQueryable<EmailAuditLog>   EmailAuditLogs      { get; }
    IQueryable<UserLogin>       UserLogins          { get; }
}