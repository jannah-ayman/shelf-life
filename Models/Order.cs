// CHANGES:
// - Made ListingID and BuyerID required; navigations required via null-forgiving.
// - Price & DeliveryFee made nullable (not needed for SWAP/DONATION; fee may be computed later).
// - Introduced required Quantity (instead of nullable RequestedQuantity) with default of 1 common in carts.
// - Removed PaymentID, RatingID, DeliveryID FKs from Order. For true 1:1, the dependents (Payment, Rating, Delivery) now hold unique FK OrderID.
// - Kept CreatedAt required and CompletedAt nullable.
// - Negotiations collection non-null and non-nullable.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public enum OrderType
    {
        SALE,
        SWAP,
        DONATION
    }

    public enum OrderStatus
    {
        PENDING,
        NEGOTIATING,
        ACCEPTED,
        REJECTED,
        PAID,
        DELIVERING,
        COMPLETED,
        CANCELLED
    }

    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public OrderType OrderType { get; set; }

        public int ListingID { get; set; }
        [ForeignKey(nameof(ListingID))]
        public BookListing Listing { get; set; } = null!;

        public int BuyerID { get; set; }
        [ForeignKey(nameof(BuyerID))]
        public User Buyer { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DeliveryFee { get; set; }

        public OrderStatus Status { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // 1:1 dependents (FKs live on the dependent)
        public Payment? Payment { get; set; }
        public Rating? Rating { get; set; }
        public Delivery? Delivery { get; set; }

        public ICollection<Negotiation> Negotiations { get; set; } = new List<Negotiation>();
    }
}
