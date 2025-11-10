namespace ShelfLife.DTOs
{
    public class OrderDto
    {
            public int OrderID { get; set; }
            public int BookListingID { get; set; }
            public string BookTitle { get; set; }
            public string BuyerName { get; set; }
            public string Status { get; set; } 
            public DateTime OrderDate { get; set; }
    }
}
