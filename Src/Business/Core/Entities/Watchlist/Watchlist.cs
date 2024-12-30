namespace ShareMarket.Core.Entities
{
    public class Watchlist : Auditable
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public long? UserId { get; set; }

        public List<WatchlistStock>? WatchlistStockList { get; set; }
    }
}
