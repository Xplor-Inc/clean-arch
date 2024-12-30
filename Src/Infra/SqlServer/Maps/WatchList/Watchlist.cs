namespace ShareMarket.SqlServer.Maps.WatchList;

public class WatchlistMap : Map<Watchlist>
{
    public override void Configure(EntityTypeBuilder<Watchlist> entity)
    {
       entity
        .Property(e => e.Name)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
         .Property(e => e.Description)
         .HasMaxLength(StaticConfiguration.MAX_LENGTH);
    }
}
