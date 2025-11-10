using Microsoft.EntityFrameworkCore;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DBcontext _context;

        public OrderRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDisplayDTO>> GetUserIncomingOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User)
                .Include(o => o.Buyer)
                .Include(o => o.Delivery)
                    .ThenInclude(d => d.DeliveryPerson)
                .Include(o => o.Payment)
                .Include(o => o.Rating)
                .Where(o => o.Listing.UserID == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDisplayDTO
                {
                    OrderID = o.OrderID,
                    OrderType = o.OrderType,
                    Status = o.Status,
                    ListingID = o.ListingID,
                    BookTitle = o.Listing.Title,
                    BookAuthor = o.Listing.Author,
                    BookCondition = o.Listing.Condition,
                    BookPhotoURL = o.Listing.PhotoURLs,
                    SellerID = o.Listing.UserID,
                    SellerName = o.Listing.User.Name,
                    SellerRating = o.Listing.User.AverageRating,
                    BuyerID = o.BuyerID,
                    BuyerName = o.Buyer.Name,
                    BuyerRating = o.Buyer.AverageRating,
                    Price = o.Price,
                    DeliveryFee = o.DeliveryFee,
                    Quantity = o.Quantity,
                    CreatedAt = o.CreatedAt,
                    CompletedAt = o.CompletedAt,
                    DeliveryStatus = o.Delivery != null ? o.Delivery.Status : (DeliveryStatus?)null,
                    PickedUpAt = o.Delivery != null ? o.Delivery.PickedUpAt : null,
                    DeliveredAt = o.Delivery != null ? o.Delivery.DeliveredAt : null,
                    DeliveryPersonName = o.Delivery != null && o.Delivery.DeliveryPerson != null
                        ? o.Delivery.DeliveryPerson.Name : null,
                    PaymentStatus = o.Payment != null ? o.Payment.Status : (PaymentStatus?)null,
                    PaidAt = o.Payment != null ? o.Payment.PaidAt : (DateTime?)null,
                    OrderScore = o.Rating != null ? o.Rating.OrderScore : (int?)null,
                    DeliveryScore = o.Rating != null ? o.Rating.DeliveryScore : (int?)null,
                    RatingComment = o.Rating != null ? o.Rating.Comment : null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDisplayDTO>> GetUserOutgoingOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User)
                .Include(o => o.Buyer)
                .Include(o => o.Delivery)
                    .ThenInclude(d => d.DeliveryPerson)
                .Include(o => o.Payment)
                .Include(o => o.Rating)
                .Where(o => o.BuyerID == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDisplayDTO
                {
                    OrderID = o.OrderID,
                    OrderType = o.OrderType,
                    Status = o.Status,
                    ListingID = o.ListingID,
                    BookTitle = o.Listing.Title,
                    BookAuthor = o.Listing.Author,
                    BookCondition = o.Listing.Condition,
                    BookPhotoURL = o.Listing.PhotoURLs,
                    SellerID = o.Listing.UserID,
                    SellerName = o.Listing.User.Name,
                    SellerRating = o.Listing.User.AverageRating,
                    BuyerID = o.BuyerID,
                    BuyerName = o.Buyer.Name,
                    BuyerRating = o.Buyer.AverageRating,
                    Price = o.Price,
                    DeliveryFee = o.DeliveryFee,
                    Quantity = o.Quantity,
                    CreatedAt = o.CreatedAt,
                    CompletedAt = o.CompletedAt,
                    DeliveryStatus = o.Delivery != null ? o.Delivery.Status : (DeliveryStatus?)null,
                    PickedUpAt = o.Delivery != null ? o.Delivery.PickedUpAt : null,
                    DeliveredAt = o.Delivery != null ? o.Delivery.DeliveredAt : null,
                    DeliveryPersonName = o.Delivery != null && o.Delivery.DeliveryPerson != null
                        ? o.Delivery.DeliveryPerson.Name : null,
                    PaymentStatus = o.Payment != null ? o.Payment.Status : (PaymentStatus?)null,
                    PaidAt = o.Payment != null ? o.Payment.PaidAt : (DateTime?)null,
                    OrderScore = o.Rating != null ? o.Rating.OrderScore : (int?)null,
                    DeliveryScore = o.Rating != null ? o.Rating.DeliveryScore : (int?)null,
                    RatingComment = o.Rating != null ? o.Rating.Comment : null
                })
                .ToListAsync();
        }

        public async Task<OrderDisplayDTO?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User)
                .Include(o => o.Buyer)
                .Include(o => o.Delivery)
                    .ThenInclude(d => d.DeliveryPerson)
                .Include(o => o.Payment)
                .Include(o => o.Rating)
                .Where(o => o.OrderID == orderId)
                .Select(o => new OrderDisplayDTO
                {
                    OrderID = o.OrderID,
                    OrderType = o.OrderType,
                    Status = o.Status,
                    ListingID = o.ListingID,
                    BookTitle = o.Listing.Title,
                    BookAuthor = o.Listing.Author,
                    BookCondition = o.Listing.Condition,
                    BookPhotoURL = o.Listing.PhotoURLs,
                    SellerID = o.Listing.UserID,
                    SellerName = o.Listing.User.Name,
                    SellerRating = o.Listing.User.AverageRating,
                    BuyerID = o.BuyerID,
                    BuyerName = o.Buyer.Name,
                    BuyerRating = o.Buyer.AverageRating,
                    Price = o.Price,
                    DeliveryFee = o.DeliveryFee,
                    Quantity = o.Quantity,
                    CreatedAt = o.CreatedAt,
                    CompletedAt = o.CompletedAt,
                    DeliveryStatus = o.Delivery != null ? o.Delivery.Status : (DeliveryStatus?)null,
                    PickedUpAt = o.Delivery != null ? o.Delivery.PickedUpAt : null,
                    DeliveredAt = o.Delivery != null ? o.Delivery.DeliveredAt : null,
                    DeliveryPersonName = o.Delivery != null && o.Delivery.DeliveryPerson != null
                        ? o.Delivery.DeliveryPerson.Name : null,
                    PaymentStatus = o.Payment != null ? o.Payment.Status : (PaymentStatus?)null,
                    PaidAt = o.Payment != null ? o.Payment.PaidAt : (DateTime?)null,
                    OrderScore = o.Rating != null ? o.Rating.OrderScore : (int?)null,
                    DeliveryScore = o.Rating != null ? o.Rating.DeliveryScore : (int?)null,
                    RatingComment = o.Rating != null ? o.Rating.Comment : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Order?> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UserOwnsOrderListingAsync(int userId, int orderId)
        {
            return await _context.Orders
                .Include(o => o.Listing)
                .AnyAsync(o => o.OrderID == orderId && o.Listing.UserID == userId);
        }

        public async Task<bool> UserIsOrderBuyerAsync(int userId, int orderId)
        {
            return await _context.Orders
                .AnyAsync(o => o.OrderID == orderId && o.BuyerID == userId);
        }
    }
}
