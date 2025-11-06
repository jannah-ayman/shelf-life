using System.ComponentModel.DataAnnotations.Schema;

namespace shelfLife.Models
{
    public enum status
    {
        Scheduled,
        InTransit,
        Delivered,
        Failed
    }
    public class Delivery
    {
        public int DeliveryID { get; set; }
        //delivary
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public virtual Request Request { get; set; }
        public string FromLocation { get; set; } = string.Empty;
        public string ToLocation { get; set; } = string.Empty;

        public string CurrentLocation {  get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryFee { get; set; }
        public status DeliveryStatus { get; set; }
        //public  int TrackingNumber { get; set; }
        public DateTime DeliveredAt { get; set; }
    }
}
