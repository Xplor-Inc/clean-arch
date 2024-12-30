using ShareMarket.Core.Entities.Schemes;

namespace ShareMarket.SqlServer.Maps.Schemes;

public class SchemeMap : Map<Scheme>
{
    public override void Configure(EntityTypeBuilder<Scheme> entity)
    {
        entity
            .ToTable("Schemes");

        entity
           .Property(e => e.SchemeCode)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
        
        entity
           .HasIndex(e => e.SchemeCode)
           .IsUnique();

        entity
           .Property(e => e.SchemeName)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.SearchId)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}