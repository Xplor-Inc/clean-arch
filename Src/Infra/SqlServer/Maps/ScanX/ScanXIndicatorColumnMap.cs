using ShareMarket.Core.Entities.ScanX;

namespace ShareMarket.SqlServer.Maps.ScanX;

public class ScanXIndicatorColumnMap : Map<ScanXIndicatorColumn>
{
    public override void Configure(EntityTypeBuilder<ScanXIndicatorColumn> entity)
    {
        entity
            .ToTable("ScanXIndicatorColumns");

        entity
            .Property(e => e.Action_1)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Indicator_1)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_2)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Indicator_2)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_3)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Indicator_3)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_4)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Indicator_4)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_5)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Indicator_5)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_6)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Indicator_6)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_7)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Indicator_7)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_8)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Indicator_8)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Action_9)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Indicator_9)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

    }
}