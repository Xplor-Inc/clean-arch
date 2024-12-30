using ShareMarket.Core.Entities.Equities;

namespace ShareMarket.SqlServer.Maps.EquityStocks;

public class EquityPriceHistoryMap : Map<EquityPriceHistory>
{
    public override void Configure(EntityTypeBuilder<EquityPriceHistory> entity)
    {
        entity
            .ToTable("EquityPriceHistories");

        entity
            .HasIndex(e => new { e.Date, e.Code })
            .IsUnique();

        entity
           .Property(e => e.Code)
           .HasMaxLength(StaticConfiguration.NAME_LENGTH);

        entity
            .HasIndex(e => e.Code);

        entity
            .HasIndex(e => e.Date);

        entity
           .Property(e => e.Name)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Avg14DaysProfit)
           .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
           .Property(e => e.Avg14DaysLoss)
           .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.Close)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.High)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.Loss)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.Low)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.Open)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.Profit)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.RS)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

        entity
            .Property(e => e.RSI)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);
    }
}