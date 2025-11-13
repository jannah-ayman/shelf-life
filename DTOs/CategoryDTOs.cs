namespace ShelfLife.DTOs
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ListingCount { get; set; }
    }

    public class CreateCategoryDTO
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCategoryDTO
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
