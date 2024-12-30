using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.ResponseCompression;
using ShareMarket.Core.Hubs;
using ShareMarket.WebApp.MinimalAPI.Accounts;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
      .AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console()
                .ReadFrom.Configuration(ctx.Configuration));
Log.Logger.Information("App is starting.....");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddSingleton<IConfigurationRoot>(builder.Configuration);


var connectionString = builder.Configuration.GetConnectionString("ShareMarket") ?? throw new InvalidOperationException("Connection string 'ShareMarket' not found.");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddLogging();
builder.Services.AddDbContextPool<ShareMarketContext>(
                         options => options.UseSqlServer(connectionString: connectionString));

// Add Hangfire services.

var connection_Hangfire = builder.Configuration.GetConnectionString("ShareMarket_Hangfire") ?? throw new InvalidOperationException("Connection string 'XplorInc' not found.");

builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connection_Hangfire, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer(z => { z.WorkerCount = Environment.ProcessorCount * 1;  z.Queues = ["default", "history", "ltp", "calculation"]; });
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2 });
//int workerCount = Environment.ProcessorCount * 1 > 10 ? 10 : Environment.ProcessorCount * 1;
//builder.Services.AddHangfireServer(z => { z.WorkerCount = workerCount; z.Queues = ["default"]; });
//builder.Services.AddHangfireServer(z => { z.WorkerCount = workerCount; z.Queues = ["history"]; });
//builder.Services.AddHangfireServer(z => { z.WorkerCount = workerCount; z.Queues = ["calculation"]; });
//builder.Services.AddHangfireServer(z => { z.WorkerCount = workerCount; z.Queues = ["ltp"]; });

// Custom Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlRepository();
builder.Services.AddCookieAuthentication(builder.Configuration);
builder.Services.AddUtilityResolver();
builder.Services.AddContexts(connectionString);
builder.Services.AddConfigurationFiles(builder.Configuration);

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddBlazorise(options =>
                {
                    options.Immediate = true;
                }).AddBootstrapProviders()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

builder.Services.AddValidatorsFromAssembly(typeof(App).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<AuditableDto>();
builder.Services.AddControllers(options =>
{
    if (!builder.Environment.IsDevelopment())
        options.Filters.Add(new RequireHttpsAttribute());
    options.Filters.Add(new ValidationFilter());
    options.EnableEndpointRouting = false;
});

builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

var app = builder.Build();
app.UseResponseCompression();
using var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
app.ConfigureSeedData(serviceScope, app.Environment, connection_Hangfire);
var version = builder.Configuration.GetSection("Version").Value;
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("App-Version", version);
    context.Response.Headers.Append("X-Frame-Options", "sameorigin");
    context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    context.Response.Headers.Append("App-Machine", Environment.MachineName);
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapHub<NotificationHub>("/notifications-hub");
app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseHangfireDashboard(options: new DashboardOptions { Authorization = [new HangfireAuthorizationFilter()] });
HangfireRecurringJobManager.CreateRecurringJob();
app.MapHangfireDashboard();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
});
// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

Log.Logger.Information($"App started successfully with version {version} on {Environment.MachineName}");
app.Run();
