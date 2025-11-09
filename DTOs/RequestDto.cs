namespace ShelfLife.DTOs
{
    public class RequestDto
    {
            public int RequestID { get; set; }
            public int BookID { get; set; }
            public string BookTitle { get; set; }
            public string RequesterName { get; set; }
            public string Status { get; set; } 
            public DateTime RequestDate { get; set; }
    }
}
