// CHANGES:
// - Collections non-null and non-nullable.
// - Kept AverageRating non-null decimal(3,2) with default 0 in code-first seeding/migrations if needed.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public enum VehicleType
    {
        BIKE,
        MOTORCYCLE,
        CAR,
        TRUCK
    }

    public class DeliveryPerson
    {
        [Key]
        public int DeliveryPersonID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? City { get; set; }

        public VehicleType VehicleType { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; }

        public int TotalDeliveries { get; set; }
        public bool IsAvailable { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalEarnings { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}
