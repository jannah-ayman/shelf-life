namespace ShelfLife.DTOs
{
    public class CreateRatingDTO
    {
        public int OrderID { get; set; }
        public int OrderScore { get; set; } 
        public int DeliveryScore { get; set; } 
        public string? Comment { get; set; }
    }
}
