using System.ComponentModel.DataAnnotations.Schema;

namespace shelfLife.Models
{
   public enum Ntype
    {
        RequestUpdate =0,
        BorrowDue =1,
        ChatNew=2
    }
    public class Notification
    {
        public int NotificationID { get; set; }
        //user
        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }
        public  Ntype Type  { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; }
        public double new_column { get; set; }

    }
}
