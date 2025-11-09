using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public class Rating
    {
        public int RatingID { get; set; }
        //user
        public int RaterID { get; set; }
        [ForeignKey(nameof(RaterID))]
        public User? Rater { get; set; }
        //listing
        public  int BookID { get; set; }
        [ForeignKey(nameof(BookID))]
        public virtual Book? Books { get; set; }
        public  int Score  { get; set; }
        public  string Comment { get; set; }
        public  DateTime CreatedAt  { get; set; }

    }
}
