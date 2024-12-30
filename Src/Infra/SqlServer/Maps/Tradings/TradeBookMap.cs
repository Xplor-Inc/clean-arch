using ShareMarket.Core.Entities.Tradings;

namespace ShareMarket.SqlServer.Maps.Tradings;

public class TradeBookMap : Map<TradeBook>
{
    public override void Configure(EntityTypeBuilder<TradeBook> entity)
    {
        entity
            .ToTable("TradeBooks");

        entity
           .Property(e => e.Code)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Strategy)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.SellAction)
           .HasConversion<string>()
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.TradingAccount)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
 
        entity
          .Property(e => e.TradeType)
          .HasConversion<string>()
          .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}