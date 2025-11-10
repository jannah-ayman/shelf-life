namespace ShelfLife.DTOs
{
    public class CreateBookListingDto
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryID { get; set; }
        public string Edition { get; set; }
        public string Description { get; set; }
    }
}
