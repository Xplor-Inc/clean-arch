namespace ShareMarket.SqlServer.Maps.WatchList
{
    public class WatchlistStockMap : Map<WatchlistStock>
    {
        public override void Configure(EntityTypeBuilder<WatchlistStock> entity)
        {
            entity.HasOne(x => x.EquityStock).WithMany(x => x.WatchlistStockList).HasForeignKey(x => x.EquityStockId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.WatchList).WithMany(x => x.WatchlistStockList).HasForeignKey(x => x.WatchListId);
        }
    }
}
