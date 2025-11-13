using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    [Table("PasswordResetToken")]
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
    }
}