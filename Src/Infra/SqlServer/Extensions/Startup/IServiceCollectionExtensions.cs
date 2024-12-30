using ShareMarket.SqlServer.Repositories;
using ShareMarket.Core.Interfaces.SqlServer;

namespace ShareMarket.SqlServer.Extensions;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSqlRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}