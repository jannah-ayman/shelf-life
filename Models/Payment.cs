// CHANGES:
// - Implemented true 1:1 by placing a unique FK `OrderID` on Payment (dependent).
// - Added [Index(OrderID, IsUnique = true)] to enforce one-to-one in DB.
// - Amount non-null, Status non-null, PaidAt non-null (set when completed).

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
