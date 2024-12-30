using ShareMarket.Core.Entities.Tradings;

namespace ShareMarket.SqlServer.Maps.Tradings;

public class TradeOrderMap : Map<TradeOrder>
{
    public override void Configure(EntityTypeBuilder<TradeOrder> entity)
    {
        entity
            .ToTable("TradeOrders");

        entity
           .Property(e => e.Code)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
        
        entity
           .Property(e => e.Remark)
           .HasMaxLength(StaticConfiguration.MAX_LENGTH);

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

        entity
          .Property(e => e.OrderStatus)
          .HasConversion<string>()
          .HasMaxLength(StaticConfiguration.NAME_LENGTH);

        entity
          .Property(e => e.OrderType)
          .HasConversion<string>()
          .HasMaxLength(StaticConfiguration.NAME_LENGTH);

        entity
          .HasOne(d => d.BuyOrder)
          .WithMany(p => p.SellOrders)
          .HasForeignKey(d => d.BuyOrderId)
          .OnDelete(DeleteBehavior.Restrict);
    }
}