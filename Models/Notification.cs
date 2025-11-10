// CHANGES:
// - Made UserID required with required navigation via null-forgiving operator.
// - Title required, Message optional, IsRead non-nullable.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public enum NotificationType
    {
        ORDER_RECEIVED,
        NEGOTIATION_MESSAGE,
        DELIVERY_ASSIGNED,
        DELIVERY_PICKED_UP,
        DELIVERY_DELIVERED,
        RATING_RECEIVED,
        ORDER_CANCELLED
    }

    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; } = null!;

        public NotificationType Type { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string? Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
