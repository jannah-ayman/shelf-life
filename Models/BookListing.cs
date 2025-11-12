using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public enum BookCondition
    {
        NEW,
        LIKE_NEW,
        VERY_GOOD,
        GOOD,
        ACCEPTABLE,
        POOR
    }

    public enum AvailabilityStatus
    {
        Available,
        Sold,
        Swapped
        // Donated removed
    }

    public class BookListing
    {
        [Key]
        public int BookListingID { get; set; }

        // Required relationship to User
        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; } = null!;

        [MaxLength(20)]
        public string? ISBN { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Author { get; set; }

        // Optional Category
        public int? CategoryID { get; set; }
        [ForeignKey(nameof(CategoryID))]
        public Category? Category { get; set; }

        public BookCondition Condition { get; set; }

        // Nullable: not needed for swaps
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [MaxLength(100)]
        public string? Edition { get; set; }

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? PhotoURLs { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public bool IsSellable { get; set; }
        // IsDonatable removed
        public bool IsSwappable { get; set; }

        public int Quantity { get; set; } = 1;
        public int AvailableQuantity { get; set; }

        public AvailabilityStatus AvailabilityStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Negotiation> Negotiations { get; set; } = new List<Negotiation>();
    }
}