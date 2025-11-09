//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ShelfLife.Models
//{
//    public enum Conditions
//    {
//        New,
//        LikeNew,
//        Good,
//        Fair,
//        Poor
//    }
//    public enum Availability
//    {

//        Private,
//        Available,
//        Pending,
//        Sold,
//        Donated,
//        Swapped,
//        Borrowed
//    }

//    public class Listings
//    {
//        [Key]
//        public int ListingID { get; set; }
//        //user
//        public int UserID { get; set; }
//        [ForeignKey(nameof(UserID))]
//        public virtual User? User { get; set; }
//        public int RatingID { get; set; }
//        [ForeignKey(nameof(RatingID))]
//        public virtual Rating? Rating { get; set; }
//        // swap 
//        //public int  OfferedListingID { get; set; }
//        //[ForeignKey(nameof(OfferedListingID))]
//        //public virtual Swap? offerd {  get; set; }
//        // book
//        public int BookID { get; set; }
//        [ForeignKey(nameof(BookID))]
//        public virtual Book? Book { get; set; }
//        public Conditions Condition { get; set; }
//        public string Notes { get; set; }
//        public int Quantity { get; set; } = 1;

//        [Column(TypeName = "decimal(18,2)")]
//        public decimal Price { get; set; }
//        public bool IsSwappable { get; set; } = true;
//        public bool IsDonatable { get; set; } = false;
//        public Availability AvailabilityStatus { get; set; }
//        public string LocationNotes { get; set; }
//        public string PhotoURLs { get; set; }
//        public DateTime CreatedAt { get; set; } = DateTime.Now;
//        public int AvailableQuantity { get; set; }
//        public ICollection<Request> requests { get; set; } = new List<Request>();
//        //public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
//        public ICollection<Swap> Swaps { get; set; } = new List<Swap>();
//    }
//}
