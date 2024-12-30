using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.SqlServer.Maps.EquityStocks;

public class EquityHistorySyncLogMap : Map<EquityHistorySyncLog>
{
    public override void Configure(EntityTypeBuilder<EquityHistorySyncLog> entity)
    {
        entity
            .ToTable("EquityHistorySyncLogs");

        entity
           .Property(e => e.Name)
           .IsRequired()
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Code)
           .IsRequired()
           .HasMaxLength(StaticConfiguration.NAME_LENGTH);

        entity
           .Property(e => e.ErrorMessage)
           .IsRequired();

        entity
           .Property(e => e.Provider)
           .IsRequired()
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}