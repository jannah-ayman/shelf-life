using Microsoft.EntityFrameworkCore;

namespace ShelfLife.Models
{
    public class DBcontext : DbContext
    {
        public DBcontext() { }
        public DBcontext(DbContextOptions<DBcontext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookListing> BookListings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryPerson> DeliveryPeople { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Negotiation> Negotiations { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER ⇄ BOOKLISTINGS (1 : Many)
            modelBuilder.Entity<BookListing>()
                .HasOne(b => b.User)
                .WithMany(u => u.BookListings)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // CATEGORY ⇄ BOOKLISTINGS (1 : Many, optional CategoryID)
            modelBuilder.Entity<BookListing>()
                .HasOne(b => b.Category)
                .WithMany(c => c.BookListings)
                .HasForeignKey(b => b.CategoryID)
                .OnDelete(DeleteBehavior.SetNull);

            // BOOKLISTING ⇄ ORDERS (1 : Many)
            modelBuilder.Entity<Order>()
               .HasOne(o => o.Listing)
               .WithMany(b => b.Orders)
               .HasForeignKey(o => o.ListingID)
               .OnDelete(DeleteBehavior.NoAction); 

            //  USER ⇄ ORDERS (1 : Many as Buyer)
            modelBuilder.Entity<Order>()
               .HasOne(o => o.Buyer)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.BuyerID)
               .OnDelete(DeleteBehavior.NoAction);

            //  ORDER ⇄ PAYMENT (1 : 1)
            // Payment is dependent (Payment holds OrderID FK)
            modelBuilder.Entity<Payment>()
               .HasOne(p => p.Order)
               .WithOne(o => o.Payment)
               .HasForeignKey<Payment>(p => p.OrderID)
               .OnDelete(DeleteBehavior.Cascade);

            //  ORDER ⇄ RATING (1 : 1)
            // Rating is dependent (Rating holds OrderID FK)
            modelBuilder.Entity<Rating>()
               .HasOne(r => r.Order)
               .WithOne(o => o.Rating)
               .HasForeignKey<Rating>(r => r.OrderID)
               .OnDelete(DeleteBehavior.Cascade);

            //  ORDER ⇄ DELIVERY (1 : 1)
            // Delivery is dependent (Delivery holds OrderID FK)
            modelBuilder.Entity<Delivery>()
               .HasOne(d => d.Order)
               .WithOne(o => o.Delivery)
               .HasForeignKey<Delivery>(d => d.OrderID)
               .OnDelete(DeleteBehavior.Cascade);

            //  DELIVERY ⇄ DELIVERY PERSON (Many : 1, optional)
            modelBuilder.Entity<Delivery>()
               .HasOne(d => d.DeliveryPerson)
               .WithMany(dp => dp.Deliveries)
               .HasForeignKey(d => d.DeliveryPersonID)
               .OnDelete(DeleteBehavior.SetNull);

            //  ORDER ⇄ NEGOTIATIONS (1 : Many)
            modelBuilder.Entity<Negotiation>()
               .HasOne(n => n.Order)
               .WithMany(o => o.Negotiations)
               .HasForeignKey(n => n.OrderID)
               .OnDelete(DeleteBehavior.Cascade);

            //  NEGOTIATION ⇄ OFFERED LISTING (optional 1 : Many)
            modelBuilder.Entity<Negotiation>()
               .HasOne(n => n.OfferedListing)
               .WithMany(b => b.Negotiations)
               .HasForeignKey(n => n.OfferedListingID)
               .OnDelete(DeleteBehavior.SetNull);

            //  USER ⇄ NOTIFICATIONS (1 : Many)
            modelBuilder.Entity<Notification>()
               .HasOne(n => n.User)
               .WithMany(u => u.Notifications)
               .HasForeignKey(n => n.UserID)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
