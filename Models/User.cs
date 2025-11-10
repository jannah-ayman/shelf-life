using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public enum UserType
    {
        NORMAL_USER,
        BUSINESS
    }

    public class User
    {
        [Key]
        public int UserID { get; set; }

        public UserType UserType { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(500)]
        public string? ProfilePhotoURL { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<BookListing> BookListings { get; set; } = new List<BookListing>();
        // Orders where this user is the Buyer
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
