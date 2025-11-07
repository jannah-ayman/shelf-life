using Microsoft.EntityFrameworkCore;

namespace ShelfLife.Models
{
    public class DBcontext : DbContext
    {
        public DBcontext() { }
        public DBcontext(DbContextOptions<DBcontext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Listings> Listings { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Swap> Swaps { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // user & rating 1:M
            modelBuilder.Entity<Rating>()
            .HasOne(r => r.Rater)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.RaterID)
            .OnDelete(DeleteBehavior.NoAction);


            // listing & rating 1:m
            modelBuilder.Entity<Rating>()
            .HasOne(r => r.Listings)
            .WithOne(l => l.Rating)
            .HasForeignKey<Listings>(r => r.RatingID)
            .OnDelete(DeleteBehavior.Cascade);

       
            //request & borrow 1:1
            modelBuilder.Entity<Request>()
           .HasOne(l => l.Borrow)
           .WithOne(r => r.Request)
           .HasForeignKey<Borrow>(r => r.RequestID)
           .OnDelete(DeleteBehavior.Cascade);

            // request & delivary 1:1

            modelBuilder.Entity<Request>()
          .HasOne(l => l.Delivery)
          .WithOne(r => r.Request)
          .HasForeignKey<Delivery>(r => r.RequestID)
          .OnDelete(DeleteBehavior.Cascade);

            //request &swap
            modelBuilder.Entity<Request>()
           .HasOne(l => l.Swap)
           .WithOne(r => r.Request)
           .HasForeignKey<Swap>(r => r.RequestID)
           .OnDelete(DeleteBehavior.Cascade);

            // Request ↔ User (Requester)
            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany(u => u.requests)
                .HasForeignKey(r => r.RequesterID)
                .OnDelete(DeleteBehavior.NoAction);


            // Message ↔ User (Sender)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderID)
                .OnDelete(DeleteBehavior.Restrict);

            // Message ↔ User (Receiver)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Swap>()
            .HasOne(s => s.swaps)              
            .WithMany(l => l.Swaps)            
            .HasForeignKey(s => s.OfferedListingID)
            .OnDelete(DeleteBehavior.NoAction);









        }
    }
}
