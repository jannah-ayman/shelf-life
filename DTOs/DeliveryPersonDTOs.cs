using ShelfLife.Models;
using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
    // DTO for delivery person profile
    public class DeliveryPersonProfileDTO
    {
        public int DeliveryPersonID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? City { get; set; }
        public VehicleType VehicleType { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalDeliveries { get; set; }
        public bool IsAvailable { get; set; }
        public decimal TotalEarnings { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO for updating delivery person profile
    public class UpdateDeliveryPersonProfileDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? City { get; set; }

        [Required]
        public VehicleType VehicleType { get; set; }
    }

    // DTO for updating availability
    public class UpdateAvailabilityDTO
    {
        [Required]
        public bool IsAvailable { get; set; }
    }

    // DTO for earnings breakdown
    public class DeliveryPersonEarningsDTO
    {
        public decimal TotalEarnings { get; set; }
        public int TotalDeliveries { get; set; }
        public decimal AverageEarningsPerDelivery { get; set; }
        public int SaleDeliveries { get; set; }
        public int SwapDeliveries { get; set; }
        public decimal ThisMonthEarnings { get; set; }
    }
}