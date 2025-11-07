using System.ComponentModel.DataAnnotations.Schema;

namespace ShelfLife.Models
{
    public class Message
    {
        public int MessageID { get; set; }
        //user
        public  int SenderID { get; set; }
        [ForeignKey(nameof(SenderID))]
        public virtual User? Sender { get; set; }
        public  int ReceiverID { get; set; }
        [ForeignKey(nameof(ReceiverID))]
        public virtual User? Receiver { get; set; }
        //requests
        public  int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }
        public string Content { get; set; }
        public  DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }


    }
}
