using ShelfLife.Models;
using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
    // DTO for detailed view of a single book listing
    public class BookListingDetailDTO
    {
        public int BookListingID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public string? Edition { get; set; }
        public BookCondition Condition { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? PhotoURLs { get; set; }
        public string? CategoryName { get; set; }
        public int? CategoryID { get; set; }
        public string? City { get; set; }
        public bool IsSellable { get; set; }
        public bool IsDonatable { get; set; }
        public bool IsSwappable { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        // Owner info
        public int UserID { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerPhone { get; set; }
        public string? OwnerEmail { get; set; }
        public decimal OwnerRating { get; set; }
        public string? OwnerCity { get; set; }
    }
    // DTO for displaying book listings (used in lists, search results)
    public class BookListingDisplayDTO
    {
        public int BookListingID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public BookCondition Condition { get; set; }
        public decimal? Price { get; set; }
        public string? CategoryName { get; set; }
        public string? City { get; set; }
        public bool IsSellable { get; set; }
        public bool IsDonatable { get; set; }
        public bool IsSwappable { get; set; }
        public int AvailableQuantity { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }
        public string? PhotoURLs { get; set; }
        public DateTime CreatedAt { get; set; }

        // Owner info (useful for public listings)
        public int UserID { get; set; }
        public string? OwnerName { get; set; }
        public decimal OwnerRating { get; set; }
    }
    // DTO for filtering/searching book listings
    public class BookListingFilterDTO
    {
        public string? SearchTerm { get; set; }
        public int? CategoryID { get; set; }
        public BookCondition? Condition { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? City { get; set; }
        public bool? IsSellable { get; set; }
        public bool? IsDonatable { get; set; }
        public bool? IsSwappable { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } // "price", "date", "title"
        public bool SortDescending { get; set; } = false;
    }
    // DTO for creating a new book listing
    public class CreateBookListingDTO
    {
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
        public int Quantity { get; set; } = 1;
    }
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