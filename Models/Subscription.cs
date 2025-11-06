using System.ComponentModel.DataAnnotations.Schema;

namespace shelfLife.Models
{
    public enum SubStatus
    {
        Active,
        Expired,
        Cancelled
    }
    public class Subscription
    {
        public int SubscriptionID { get; set; }
        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubStatus Status { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyFee { get; set; }
    }
}
