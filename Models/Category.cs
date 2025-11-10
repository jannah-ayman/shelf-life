using System.ComponentModel.DataAnnotations;

namespace ShelfLife.Models
{
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
