using ShareMarket.Core.Interfaces.Utility.Security;
using ShareMarket.Core.Models.Configurations;

namespace ShareMarket.WebApp.Extensions;
public static class IServiceCollectionExtension
{
    public static void AddCookieAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var cookieConfig = configuration.GetSection("Authentication:Cookie").Get<CookieAuthenticationConfiguration>() ?? throw new NullReferenceException("Provide Authentication:Cookie section in AppSettings.json");
        services.AddSingleton((sp) => cookieConfig);

        var cookie = new CookieBuilder()
        {
            Name = cookieConfig.CookieName,
            SameSite = SameSiteMode.Strict
        };
        var cookieEvents = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            },
            OnValidatePrincipal = PrincipalValidator.ValidateAsync
        };

        services.AddAuthentication(cookieConfig.AuthenticationScheme)
            .AddCookie(cookieConfig.AuthenticationScheme, options =>
            {
                options.Cookie = cookie;
                options.Events = cookieEvents;
            });
    }

    public static void AddContexts(this IServiceCollection services, string connectionString)
    {
        var loggerFactory = new Serilog.Extensions.Logging.SerilogLoggerFactory(Log.Logger, false);
        services.AddScoped<IUserIdentityProcessor, UserIdentityProcessor>();
        services.AddDbContext<ShareMarketContext>(ServiceLifetime.Scoped);
        services.AddScoped((sp) => new ShareMarketContext(connectionString, loggerFactory));
        services.AddScoped<DataContext<User>>((sp) => new ShareMarketContext(connectionString, loggerFactory));
        services.AddScoped<IDataContext<User>>((sp) => new ShareMarketContext(connectionString, loggerFactory));
        services.AddScoped<IContext>((sp) => new ShareMarketContext(connectionString, loggerFactory));
        services.AddScoped<IShareMarketContext>((sp) => new ShareMarketContext(connectionString, loggerFactory));
    }

    public static void AddConfigurationFiles(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var emailSection = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>() ?? throw new NullReferenceException("Provide EmailConfiguration section in AppSettings.json");
        services.AddSingleton((sp) => emailSection);
    }
}