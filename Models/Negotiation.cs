using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public class Negotiation
    {
        [Key]
        public int NegotiationID { get; set; }

        public int OrderID { get; set; }
        [ForeignKey(nameof(OrderID))]
        public Order Order { get; set; } = null!;

        public int? OfferedListingID { get; set; }
        [ForeignKey(nameof(OfferedListingID))]
        public BookListing? OfferedListing { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
