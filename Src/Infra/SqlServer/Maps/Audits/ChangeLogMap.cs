using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShareMarket.SqlServer.Maps.Audits;
public class ChangeLogMap : Map<ChangeLog>
{
    public override void Configure(EntityTypeBuilder<ChangeLog> entity)
    {
        entity
            .ToTable("ChangeLogs");
        
        entity
            .Property(e => e.EntityName)
            .IsRequired()
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.NewValue)
            .HasMaxLength(StaticConfiguration.MAX_LENGTH);

        entity
            .Property(e => e.OldValue)
            .HasMaxLength(StaticConfiguration.MAX_LENGTH);

        entity
            .Property(e => e.PropertyName)
            .IsRequired()
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

    }
}