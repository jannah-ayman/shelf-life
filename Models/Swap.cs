using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public class Swap
    {
        public int SwapID { get; set; }
        //request 
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }
        public int OfferedListingID { get; set; }
        [ForeignKey(nameof(OfferedListingID))]
        public virtual Listings? swaps { get; set; }
        public int OfferedQuantity { get; set; }
        public string NegotiationNotes { get; set; }
        //public ICollection<Listings>listings { get; set; } =new List<Listings>();
                
    }
}
