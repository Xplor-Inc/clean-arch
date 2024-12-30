using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.SqlServer.Maps.EquityStocks;

public class EquityStockMap : Map<EquityStock>
{
    public override void Configure(EntityTypeBuilder<EquityStock> entity)
    {
        entity
            .ToTable("EquityStocks");

        entity
           .Property(e => e.Code)
           .HasMaxLength(StaticConfiguration.NAME_LENGTH);

        entity
            .HasIndex(e => e.Code).IsUnique();

        entity
           .Property(e => e.Name)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Industry)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.GrowwSearchId)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Sector)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Isin)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.DayHigh)
           .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.DayLow)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.LTP)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(StaticConfiguration.NAME_LENGTH);


        entity.HasOne(x => x.EquityStockCalculation).WithOne(x => x.EquityStock).HasForeignKey<EquityStock>(x=>x.EquityStockCalculationId);

    }
}