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
