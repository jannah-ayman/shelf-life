using System.ComponentModel.DataAnnotations.Schema;

namespace shelfLife.Models
{
    public class Swap
    {
        public int SwapID { get; set; }
        //request 
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public virtual Request Request { get; set; }
        public long OfferedListingID { get; set; }
        public int OfferedQuantity { get; set; }
        public string NegotiationNotes { get; set; }
        public ICollection<Listings>listings { get; set; } =new List<Listings>();
                
    }
}
