using System.ComponentModel.DataAnnotations;

namespace ShelfLife.Models
{
    public enum Utype
    {
        Normal,
        Business
    }
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string Email { get; set; }
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public Utype UserType { get; set; }
        [MaxLength(100)]
        public string ProfileName { get; set; }
        public string ProfileDescription { get; set; }
        public string ProfilePhotoURL { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        //rating 
       
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>(); 
        public ICollection<Subscription> subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Request> requests { get; set; } = new List<Request>();
        public ICollection<Listings> listings { get; set; } = new List<Listings>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        //modifide from database
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        //public object Rating { get; internal set; }
    }
}
