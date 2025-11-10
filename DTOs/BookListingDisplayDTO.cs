using System.ComponentModel.DataAnnotations;
using ShelfLife.Models;

namespace ShelfLife.DTOs
{
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
}