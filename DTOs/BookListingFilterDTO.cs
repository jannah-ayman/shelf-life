using ShelfLife.Models;

namespace ShelfLife.DTOs
{
    // DTO for filtering/searching book listings
    public class BookListingFilterDTO
    {
        public string? SearchTerm { get; set; }
        public int? CategoryID { get; set; }
        public BookCondition? Condition { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? City { get; set; }
        public bool? IsSellable { get; set; }
        public bool? IsDonatable { get; set; }
        public bool? IsSwappable { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } // "price", "date", "title"
        public bool SortDescending { get; set; } = false;
    }
}
