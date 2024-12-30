using ShareMarket.Core.Entities.Tradings;

namespace ShareMarket.SqlServer.Maps.Tradings;

public class VirtualTradeMap : Map<VirtualTrade>
{
    public override void Configure(EntityTypeBuilder<VirtualTrade> entity)
    {
        entity
            .ToTable("VirtualTradings");

        entity
        .Property(e => e.Name)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Code)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Stratergy)
           .HasConversion<string>()
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}