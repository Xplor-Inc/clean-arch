namespace ShareMarket.Core.Models.Dtos
{
    public class WatchlistDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public long? UserId { get; set; }

        public List<WatchlistStockDto>? WatchlistStockList { get; set; }
    }
}
