using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
   public enum Bstatus
    {
        Active=0,
        OnHold=1,
        Overdue=2
    }
    public class Borrow
    {
        public int BorrowID { get; set; }
        //request
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request {  get; set; }
        public DateTime DueDate { get; set; }
        public DateTime BorrowStartDate { get; set; }
        public int BorrowedQuantity { get; set; } = 1;
        public Bstatus Status { get; set; }
    }
}
