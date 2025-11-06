using System.ComponentModel.DataAnnotations.Schema;

namespace shelfLife.Models
{
    public enum reqType
    {
        Pending,
        Accepted,
        Rejected,
        Completed,
        Cancelled
    }
    public class Request
    {
        public int RequestID { get; set; }
        //user
        public int RequesterID { get; set; }
        [ForeignKey(nameof(RequesterID))]
        public virtual User User { get; set; } 
        //listings
        public int ListingID { get; set; }
        [ForeignKey(nameof(ListingID))]
        public virtual Listings Listings { get; set; }
        //Borrow
        public int BorrowID { get; set; }
        [ForeignKey(nameof(BorrowID))]
        public virtual Borrow Borrow { get; set; }
        //delivary
        public int DeliveryID { get; set; }
        [ForeignKey(nameof(DeliveryID))]
        public virtual Delivery Delivery { get; set; }
        //swap
        public int SwapID { get; set; }
        [ForeignKey(nameof(SwapID))]
        public virtual Swap Swap { get; set; }
        public reqType RequestType { get; set; } 
        public long Status { get; set; } 
        public int RequestedQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Message> Messages { get; set; }= new List<Message>();
    }
}
