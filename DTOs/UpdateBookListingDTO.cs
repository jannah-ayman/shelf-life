using ShelfLife.Models;
using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
    // DTO for updating an existing book listing
    public class UpdateBookListingDTO
    {
        [Required]
        public int BookListingID { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Author { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        public int? CategoryID { get; set; }

        [Required]
        public BookCondition Condition { get; set; }

        [Range(0, 999999.99)]
        public decimal? Price { get; set; }

        [MaxLength(100)]
        public string? Edition { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? PhotoURLs { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public bool IsSellable { get; set; }
        public bool IsDonatable { get; set; }
        public bool IsSwappable { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        public AvailabilityStatus AvailabilityStatus { get; set; }
    }
}
