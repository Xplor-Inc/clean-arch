using ShareMarket.Core.Entities.ScanX;

namespace ShareMarket.SqlServer.Maps.ScanX;

public class ScanXEquityMap : Map<ScanXEquity>
{
    public override void Configure(EntityTypeBuilder<ScanXEquity> entity)
    {
        entity
            .ToTable("ScanXEquities");

        entity
        .Property(e => e.DispSym)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.Exch)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.Inst)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.Isin)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.Sector)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.Seg)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.SubSector)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.Seosym)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
        .Property(e => e.Sym)
        .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}