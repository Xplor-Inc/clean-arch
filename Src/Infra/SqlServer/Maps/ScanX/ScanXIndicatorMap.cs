using ShareMarket.Core.Entities.ScanX;

namespace ShareMarket.SqlServer.Maps.ScanX;

public class ScanXIndicatorMap : Map<ScanXIndicator>
{
    public override void Configure(EntityTypeBuilder<ScanXIndicator> entity)
    {
        entity
            .ToTable("ScanXIndicators");

        entity
            .Property(e => e.Action)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Indicator)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Group)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}