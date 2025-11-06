using System.ComponentModel.DataAnnotations.Schema;

namespace shelfLife.Models
{
    public class Rating
    {
        public int RatingID { get; set; }
        //user
        public int RaterID { get; set; }
        [ForeignKey(nameof(RaterID))]
        public User Rater { get; set; }
        //listing
        public  int ListingID { get; set; }
        [ForeignKey(nameof(ListingID))]
        public virtual Listings Listings { get; set; }
        public  int Score  { get; set; }
        public  string Comment { get; set; }
        public  DateTime CreatedAt  { get; set; }

    }
}
