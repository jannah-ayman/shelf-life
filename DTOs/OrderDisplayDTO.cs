using ShelfLife.Models;

namespace ShelfLife.DTOs
{
    // DTO for displaying orders (both incoming and outgoing)
    public class OrderDisplayDTO
    {
        public int OrderID { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus Status { get; set; }

        // Book listing info
        public int ListingID { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookAuthor { get; set; }
        public BookCondition BookCondition { get; set; }
        public string? BookPhotoURL { get; set; }

        // Seller info
        public int SellerID { get; set; }
        public string? SellerName { get; set; }
        public decimal SellerRating { get; set; }

        // Buyer info
        public int BuyerID { get; set; }
        public string? BuyerName { get; set; }
        public decimal BuyerRating { get; set; }

        // Transaction details
        public decimal? Price { get; set; }
        public decimal? DeliveryFee { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Delivery info
        public DeliveryStatus? DeliveryStatus { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? DeliveryPersonName { get; set; }

        // Payment info
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? PaidAt { get; set; }

        // Rating info
        public int? OrderScore { get; set; }
        public int? DeliveryScore { get; set; }
        public string? RatingComment { get; set; }
    }
}
