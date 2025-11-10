using ShelfLife.Models;

namespace ShelfLife.DTOs
{
    public class DeliveryDTO
    {
        public int DeliveryID { get; set; }
        public int OrderID { get; set; }
        public string PickupAddress { get; set; } = string.Empty;
        public string PickupPhone { get; set; } = string.Empty;
        public string DropoffAddress { get; set; } = string.Empty;
        public string DropoffPhone { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? DeliveryPersonName { get; set; }
        public string? DeliveryPersonPhone { get; set; }
        public decimal? DeliveryPersonRating { get; set; }
    }
}
