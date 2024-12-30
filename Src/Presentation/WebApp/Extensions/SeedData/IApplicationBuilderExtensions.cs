using ShareMarket.Core.Interfaces.Utility.Security;

namespace ShareMarket.WebApp.Extensions.SeedData;
public static class IApplicationBuilderExtensions
{
    public static void ConfigureSeedData(this IApplicationBuilder _, IServiceScope serviceScope,IWebHostEnvironment Env, string connection_Hangfire)
    {
        var context = serviceScope.ServiceProvider.GetService<ShareMarketContext>() ?? throw new InvalidOperationException("context");
        context.Database.SetCommandTimeout(10000);
        context.Database.Migrate();

        //var share = new ShareMarketContext(connection_Hangfire, null);
        //string query = File.ReadAllText(Path.Combine(Env.WebRootPath, "scripts", "HangfireScript.sql"));
        //share.Database.ExecuteSqlRaw(query);

        var encryption = serviceScope.ServiceProvider.GetService<IEncryption>();
        if (encryption != null)
            context.AddInitialData(encryption);
    }
}