using ShelfLife.Models;
using System.ComponentModel.DataAnnotations;

namespace ShelfLife.DTOs
{
    public class UserRegisterDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Location { get; set; }
        public string? City { get; set; }

        // Organization fields
        public string? OrganizationName { get; set; }
        public string? OrganizationDetails { get; set; }

        [Required]
        public UserType UserType { get; set; } = UserType.NORMAL_USER;
    }
}
