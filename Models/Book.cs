using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public enum Conditions
    {
        New,
        LikeNew,
        Good,
        Fair,
        Poor
    }

    public enum Availability
    {
        Private,
        Available,
        Pending,
        Sold,
        Donated,
        Swapped,
        Borrowed
    }

    public class Book
    {
        [Key]
        public int BookID { get; set; }

        
        public string ISBN { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

       


        public string Publisher { get; set; }

        public string Edition { get; set; }

        public string Description { get; set; }

         public int CategoryID { get; set; }
        [ForeignKey(nameof(CategoryID))]
        public virtual Category ? Category { get; set; }
        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public virtual User? User { get; set; }

        public int RatingID { get; set; }
        [ForeignKey(nameof(RatingID))]
        public virtual Rating? Rating { get; set; }

        public Conditions Condition { get; set; }
        public string Notes { get; set; }
        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsSwappable { get; set; } = true;
        public bool IsDonatable { get; set; } = false;

        public Availability AvailabilityStatus { get; set; }

        public string LocationNotes { get; set; }
        public string PhotoURLs { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int AvailableQuantity { get; set; }

        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
        public virtual ICollection<Swap> Swaps { get; set; } = new List<Swap>();
    }
}
