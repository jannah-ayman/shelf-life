using ShelfLife.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Swap
{
    public int SwapID { get; set; }

   
    public int RequestID { get; set; }
    [ForeignKey(nameof(RequestID))]
    public virtual Request? Request { get; set; }

    public int OfferedBookID { get; set; }
    [ForeignKey(nameof(OfferedBookID))]
    public virtual Book? OfferedBook { get; set; }

    public int OfferedQuantity { get; set; }
    public string NegotiationNotes { get; set; }
}
