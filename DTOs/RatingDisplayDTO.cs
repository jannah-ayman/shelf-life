namespace ShelfLife.DTOs
{
    public class RatingDisplayDTO
    {
        public int RatingID { get; set; }
        public int OrderID { get; set; }
        public int OrderScore { get; set; }
        public int DeliveryScore { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

       
        public string? SellerName { get; set; }
        public string? BuyerName { get; set; }
    }
}
