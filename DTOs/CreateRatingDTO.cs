using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
    public class CreateRatingDTO
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Order score must be between 1 and 5 stars")]
        public int OrderScore { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Delivery score must be between 1 and 5 stars")]
        public int DeliveryScore { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}