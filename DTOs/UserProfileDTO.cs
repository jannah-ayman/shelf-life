using ShelfLife.Models;

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
}
