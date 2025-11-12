using ShelfLife.Models;
using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
    public class UserProfileDTO
    {
        public int UserID { get; set; }
        public UserType UserType { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ProfilePhotoURL { get; set; }
        public decimal AverageRating { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalListings { get; set; }
        public int TotalOrders { get; set; }
    }

    // DTO for updating user profile
    public class UpdateUserProfileDTO
    {
        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(500)]
        public string? ProfilePhotoURL { get; set; }
    }

    public class UserDashboardStatsDTO
    {
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int SoldItems { get; set; }
        public int SwappedItems { get; set; }
        public int IncomingOrders { get; set; }
        public int OutgoingOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal AverageRating { get; set; }
    }
}