using ShareMarket.Core.Entities.Schemes;

namespace ShareMarket.SqlServer.Maps.Schemes;

public class SchemeEquityHoldingMap : Map<SchemeEquityHolding>
{
    public override void Configure(EntityTypeBuilder<SchemeEquityHolding> entity)
    {
        entity
            .ToTable("SchemeEquityHoldings");

        entity
            .HasIndex(e => new { e.SchemeCode, e.Code })
            .IsUnique();

        entity
            .Property(e => e.Code)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.CompanyName)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.InstrumentName)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.MarketCap)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.MarketValue)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.NatureName)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.Rating)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.RatingMarketCap)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.SchemeCode)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.SectorName)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
           .Property(e => e.StockSearchId)
           .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}