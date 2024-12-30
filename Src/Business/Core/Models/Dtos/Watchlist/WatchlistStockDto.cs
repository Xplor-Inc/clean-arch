using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Models.Dtos.Equities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMarket.Core.Models.Dtos
{
    public class WatchlistStockDto:AuditableDto
    {
        public long WatchListId { get; set; }
        public long StockId { get; set; }
        public EquityStockDto? EquityStock { get; set; }
        public WatchlistDto? WatchList { get; set; }
    }
}
