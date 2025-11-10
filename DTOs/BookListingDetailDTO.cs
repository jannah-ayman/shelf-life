using ShelfLife.Models;

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

}
