namespace ShareMarket.WebApp
{
    public class SelectListItem
    {
        public required string  Key         { get; set; }
        public required string  Value       { get; set; }
        public bool     Selected    { get; set; }
        public int      Count       { get; set; }
    }
}
