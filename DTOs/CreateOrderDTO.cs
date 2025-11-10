using ShelfLife.Models;
using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
    // DTO for creating an order
    public class CreateOrderDTO
    {
        [Required]
        public int ListingID { get; set; }

        [Required]
        public OrderType OrderType { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        // For negotiations/swaps
        public int? OfferedListingID { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }

        // Delivery addresses (for non-donation or donation with delivery)
        [MaxLength(500)]
        public string? DropoffAddress { get; set; }

        [MaxLength(20)]
        public string? DropoffPhone { get; set; }
    }
}
