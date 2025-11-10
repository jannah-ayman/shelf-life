namespace ShelfLife.DTOs
{
    public class UserDashboardStatsDTO
    {
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int SoldItems { get; set; }
        public int DonatedItems { get; set; }
        public int SwappedItems { get; set; }
        public int IncomingOrders { get; set; }
        public int OutgoingOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal AverageRating { get; set; }
    }
}
