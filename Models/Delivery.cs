using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShelfLife.Models
{
    public enum DeliveryStatus
    {
        PENDING,
        ASSIGNED,
        PICKED_UP,
        DELIVERED,
        FAILED
    }

    [Index(nameof(OrderID), IsUnique = true)]
    public class Delivery
    {
        [Key]
        public int DeliveryID { get; set; }

        public int? DeliveryPersonID { get; set; }
        [ForeignKey(nameof(DeliveryPersonID))]
        public DeliveryPerson? DeliveryPerson { get; set; }

        [MaxLength(500)]
        public string PickupAddress { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PickupPhone { get; set; } = string.Empty;

        [MaxLength(500)]
        public string DropoffAddress { get; set; } = string.Empty;

        [MaxLength(20)]
        public string DropoffPhone { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryFee { get; set; }

        public DeliveryStatus Status { get; set; }

        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // 1:1 with Order (Delivery is dependent)
        [ForeignKey(nameof(Order))]
        public int OrderID { get; set; }
        public Order Order { get; set; } = null!;
    }
}
