using ShareMarket.Core.Entities.Equities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareMarket.Core.Entities
{
    public class WatchlistStock:Auditable
    {
        public long WatchListId { get; set; }
        public long EquityStockId { get; set; }
        public EquityStock? EquityStock { get; set; }
        public Watchlist? WatchList { get; set; }
       
    }
}
