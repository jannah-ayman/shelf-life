// CHANGES:
// - Implemented true 1:1 by placing unique FK `OrderID` on Rating (dependent).
// - Added [Index(OrderID, IsUnique = true)].
// - Kept Comment optional, CreatedAt required.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShelfLife.Models
{
    [Index(nameof(OrderID), IsUnique = true)]
    public class Rating
    {
        [Key]
        public int RatingID { get; set; }

        public int OrderScore { get; set; }
        public int DeliveryScore { get; set; }

        [Column(TypeName = "text")]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        // 1:1 with Order (Rating is dependent)
        [ForeignKey(nameof(Order))]
        public int OrderID { get; set; }
        public Order Order { get; set; } = null!;
    }
}
