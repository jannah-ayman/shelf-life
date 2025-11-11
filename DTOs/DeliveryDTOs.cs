using ShelfLife.Models;

namespace ShelfLife.DTOs
{
    public class DeliveryDTO
    {
        public int DeliveryID { get; set; }
        public int OrderID { get; set; }
        public string PickupAddress { get; set; } = string.Empty;
        public string PickupPhone { get; set; } = string.Empty;
        public string DropoffAddress { get; set; } = string.Empty;
        public string DropoffPhone { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? DeliveryPersonName { get; set; }
        public string? DeliveryPersonPhone { get; set; }
        public decimal? DeliveryPersonRating { get; set; }
    }
    // DTO for filtering available delivery orders
    public class DeliveryOrderFilterDTO
    {
        public string? City { get; set; }
        public decimal? MaxDistance { get; set; } // in km
        public decimal? MinFee { get; set; }
        public decimal? MaxFee { get; set; }
        public OrderType? OrderType { get; set; }
    }

    // DTO for displaying available orders to delivery persons
    public class DeliveryOrderDisplayDTO
    {
        public int OrderID { get; set; }
        public OrderType OrderType { get; set; }

        // Book info
        public string BookTitle { get; set; } = string.Empty;
        public string? BookAuthor { get; set; }
        public string? BookPhotoURL { get; set; }

        // Pickup info (seller location)
        public string PickupAddress { get; set; } = string.Empty;
        public string PickupPhone { get; set; } = string.Empty;
        public string? PickupCity { get; set; }

        // Dropoff info (buyer location)
        public string DropoffAddress { get; set; } = string.Empty;
        public string DropoffPhone { get; set; } = string.Empty;
        public string? DropoffCity { get; set; }

        // Delivery details
        public decimal DeliveryFee { get; set; }
        public decimal? EstimatedDistance { get; set; } // in km

        public DateTime CreatedAt { get; set; }
    }

    // DTO for detailed delivery information
    public class DeliveryDetailDTO
    {
        public int DeliveryID { get; set; }
        public int OrderID { get; set; }
        public int DeliveryPersonID { get; set; }
        public OrderType OrderType { get; set; }
        public DeliveryStatus Status { get; set; }

        // Book info
        public string BookTitle { get; set; } = string.Empty;
        public string? BookAuthor { get; set; }
        public string? BookPhotoURL { get; set; }
        public int Quantity { get; set; }

        // Pickup info
        public string PickupAddress { get; set; } = string.Empty;
        public string PickupPhone { get; set; } = string.Empty;
        public string? PickupCity { get; set; }
        public int SellerID { get; set; }
        public string? SellerName { get; set; }

        // Dropoff info
        public string DropoffAddress { get; set; } = string.Empty;
        public string DropoffPhone { get; set; } = string.Empty;
        public string? DropoffCity { get; set; }
        public int BuyerID { get; set; }
        public string? BuyerName { get; set; }

        // Delivery details
        public decimal DeliveryFee { get; set; }
        public decimal? EstimatedDistance { get; set; }

        // Confirmation flags
        public bool DeliveryPersonConfirmed { get; set; }
        public bool BuyerConfirmed { get; set; }

        // Timestamps
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO for delivery person statistics
    public class DeliveryPersonStatsDTO
    {
        public int TotalDeliveries { get; set; }
        public int CompletedDeliveries { get; set; }
        public int ActiveDeliveries { get; set; }
        public int CancelledDeliveries { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

    // DTO for requesting delivery (used by buyer for donations)
    public class RequestDeliveryDTO
    {
        public int OrderID { get; set; }
        public string DropoffAddress { get; set; } = string.Empty;
        public string DropoffPhone { get; set; } = string.Empty;
    }
}