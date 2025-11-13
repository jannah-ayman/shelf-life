using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShelfLife.Models
{
    public enum PaymentStatus
    {
        PENDING,
        COMPLETED
    }
    [Table("Payment")]
    [Index(nameof(OrderID), IsUnique = true)]
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime PaidAt { get; set; }

        // 1:1 with Order (Payment is dependent)
        [ForeignKey(nameof(Order))]
        public int OrderID { get; set; }
        public Order Order { get; set; } = null!;
    }
}
