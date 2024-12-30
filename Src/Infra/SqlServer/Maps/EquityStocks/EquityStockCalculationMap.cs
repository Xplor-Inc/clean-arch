using ShareMarket.Core.Entities.Equities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMarket.SqlServer.Maps.EquityStocks
{
    public class EquityStockCalculationMap : Map<EquityStockCalculation>
    {
        public override void Configure(EntityTypeBuilder<EquityStockCalculation> entity)
        {
            entity
            .Property(e => e.YearHigh)
            .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

            entity
                .Property(e => e.YearLow)
                .HasPrecision(StaticConfiguration.DECIMAL_PRECISION, StaticConfiguration.DECIMAL_SCALE_2);

            entity.HasOne(x => x.EquityStock).WithOne(x => x.EquityStockCalculation).HasForeignKey<EquityStockCalculation>(x=>x.EquityStockId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
