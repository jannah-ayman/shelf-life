using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public ICollection<BookListing> BookListings { get; set; } = new List<BookListing>();
    }
}
