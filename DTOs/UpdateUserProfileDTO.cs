using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
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
}
