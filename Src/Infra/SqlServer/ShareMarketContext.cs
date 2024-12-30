using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Entities.ScanX;
using ShareMarket.Core.Entities.Schemes;
using ShareMarket.Core.Entities.Settings;
using ShareMarket.Core.Entities.Tradings;
using ShareMarket.Core.Interfaces.SqlServer;
using ShareMarket.SqlServer.Extensions;
using ShareMarket.SqlServer.Maps.Audits;
using ShareMarket.SqlServer.Maps.EquityStocks;
using ShareMarket.SqlServer.Maps.ScanX;
using ShareMarket.SqlServer.Maps.Schemes;
using ShareMarket.SqlServer.Maps.Tradings;
using ShareMarket.SqlServer.Maps.Users;
using ShareMarket.SqlServer.Maps.WatchList;

namespace ShareMarket.SqlServer;
public class ShareMarketContext(string connectionString, ILoggerFactory? loggerFactory) 
    : DataContext<User>(connectionString, loggerFactory), IShareMarketContext
{
    #region Properties
    public DbSet<EmailAuditLog>         EmailAuditLogs          { get; set; }
    public DbSet<UserLogin>             UserLogins              { get; set; }
    public DbSet<EquityPriceHistory>    EquityPriceHistories    { get; set; }
    public DbSet<EquityStock>           EquityStocks            { get; set; }
    public DbSet<Scheme>                Schemes                 { get; set; }
    public DbSet<SchemeEquityHolding>   SchemeEquityHoldings    { get; set; }
    public DbSet<EquityHistorySyncLog>  EquityHistorySyncLogs   { get; set; }
    public DbSet<VirtualTrade>          VirtualTrades           { get; set; }
    public DbSet<TradeBook>             TradeBooks              { get; set; }
    public DbSet<TradeOrder>            TradeOrders             { get; set; }
    public DbSet<Watchlist>             Watchlist               { get; set; }
    public DbSet<WatchlistStock>        WatchlistStock          { get; set; }
    public DbSet<EquityStockCalculation> EquityStockCalculation { get; set; }
    public DbSet<AppConfig>             AppConfigs              { get; set; }
    public DbSet<ScanXEquity>           ScanXEquities           { get; set; }
    public DbSet<ScanXIndicator>        ScanXIndicators         { get; set; }
    public DbSet<ScanXIndicatorColumn>  ScanXIndicatorColumns   { get; set; }

    #endregion
  
    #region Constructor

    #endregion

    #region IShareMarketContext Implementation
    IQueryable<EmailAuditLog>           IShareMarketContext.EmailAuditLogs         => EmailAuditLogs;
    IQueryable<ChangeLog>               IShareMarketContext.ChangeLogs             => ChangeLogs;
    IQueryable<UserLogin>               IShareMarketContext.UserLogins             => UserLogins;

    #endregion

    public override void ConfigureMappings(ModelBuilder modelBuilder)
    {
        modelBuilder.AddMapping(new EquityHistorySyncLogMap());
        modelBuilder.AddMapping(new EquityPriceHistoryMap());
        modelBuilder.AddMapping(new EquityStockMap());
        modelBuilder.AddMapping(new SchemeEquityHoldingMap());
        modelBuilder.AddMapping(new SchemeMap());
        modelBuilder.AddMapping(new VirtualTradeMap());
        modelBuilder.AddMapping(new TradeBookMap());
        modelBuilder.AddMapping(new ChangeLogMap());
        modelBuilder.AddMapping(new EmailAuditLogMap());
        modelBuilder.AddMapping(new UserMap());
        modelBuilder.AddMapping(new UserLoginMap()); 
        modelBuilder.AddMapping(new WatchlistMap());
        modelBuilder.AddMapping(new WatchlistStockMap());
        modelBuilder.AddMapping(new EquityStockCalculationMap());
        modelBuilder.AddMapping(new ScanXEquityMap());
        modelBuilder.AddMapping(new ScanXIndicatorMap());
        modelBuilder.AddMapping(new ScanXIndicatorColumnMap());
        modelBuilder.AddMapping(new TradeOrderMap());
        base.ConfigureMappings(modelBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}