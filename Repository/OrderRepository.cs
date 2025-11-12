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

        // Existing retrieval methods
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
                .Select(o => MapToOrderDisplayDTO(o))
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
                .Select(o => MapToOrderDisplayDTO(o))
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
                .Select(o => MapToOrderDisplayDTO(o))
                .FirstOrDefaultAsync();
        }

        // Basic CRUD
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

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        // Ownership checks
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

        // Helper methods
        public async Task<BookListing?> GetListingByIdAsync(int listingId)
        {
            return await _context.BookListings
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.BookListingID == listingId);
        }

        public async Task<bool> CanUserSwapAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null && user.UserType == UserType.NORMAL_USER;
        }

        public async Task<bool> IsListingAvailableForOrderAsync(int listingId, int quantity)
        {
            var listing = await _context.BookListings.FindAsync(listingId);
            return listing != null &&
                   listing.AvailableQuantity >= quantity &&
                   listing.AvailabilityStatus == AvailabilityStatus.Available;
        }

        // Sale order creation - Auto-accepted
        public async Task<Order?> CreateSaleOrderAsync(int buyerId, CreateSaleOrderDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var listing = await GetListingByIdAsync(dto.ListingID);
                if (listing == null)
                    return null;

                // Validations
                if (!listing.IsSellable || listing.Price == null)
                    return null;

                if (!await IsListingAvailableForOrderAsync(dto.ListingID, dto.Quantity))
                    return null;

                if (listing.UserID == buyerId)
                    return null;

                // Calculate fees
                decimal basePrice = listing.Price.Value * dto.Quantity;
                decimal platformFee = basePrice * 0.05m; // 5% platform fee
                decimal deliveryFee = !string.IsNullOrWhiteSpace(dto.DropoffAddress) ? 50.00m : 0m;
                decimal totalPrice = basePrice + platformFee + deliveryFee;

                // Create order with ACCEPTED status (auto-accepted for sales)
                var order = new Order
                {
                    OrderType = OrderType.SALE,
                    ListingID = dto.ListingID,
                    BuyerID = buyerId,
                    Price = basePrice,
                    DeliveryFee = deliveryFee,
                    Status = OrderStatus.ACCEPTED, // Auto-accepted
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create payment record
                var payment = new Payment
                {
                    OrderID = order.OrderID,
                    Amount = totalPrice,
                    Status = PaymentStatus.PENDING,
                    PaidAt = DateTime.UtcNow
                };
                _context.Payments.Add(payment);

                // Create delivery record if address provided
                if (!string.IsNullOrWhiteSpace(dto.DropoffAddress))
                {
                    var delivery = new Delivery
                    {
                        OrderID = order.OrderID,
                        PickupAddress = listing.User.Address ?? string.Empty,
                        PickupPhone = listing.User.Phone ?? string.Empty,
                        DropoffAddress = dto.DropoffAddress,
                        DropoffPhone = dto.DropoffPhone ?? string.Empty,
                        DeliveryFee = deliveryFee,
                        Status = DeliveryStatus.ASSIGNED,
                        DeliveryPersonConfirmed = false,
                        BuyerConfirmed = false
                    };
                    _context.Deliveries.Add(delivery);
                }

                // Reserve quantity
                listing.AvailableQuantity -= dto.Quantity;
                _context.BookListings.Update(listing);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Swap order creation - Starts with NEGOTIATING status
        public async Task<Order?> CreateSwapOrderAsync(int buyerId, CreateSwapOrderDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!await CanUserSwapAsync(buyerId))
                    return null;

                var requestedListing = await GetListingByIdAsync(dto.ListingID);
                var offeredListing = await GetListingByIdAsync(dto.OfferedListingID);

                if (requestedListing == null || offeredListing == null)
                    return null;

                // Validations
                if (!requestedListing.IsSwappable || !offeredListing.IsSwappable)
                    return null;

                if (!await IsListingAvailableForOrderAsync(dto.ListingID, dto.Quantity))
                    return null;

                if (!await IsListingAvailableForOrderAsync(dto.OfferedListingID, dto.Quantity))
                    return null;

                if (offeredListing.UserID != buyerId)
                    return null;

                if (requestedListing.UserID == buyerId)
                    return null;

                // Calculate delivery fee
                decimal deliveryFee = !string.IsNullOrWhiteSpace(dto.DropoffAddress) ? 50.00m : 0m;

                // Create order with NEGOTIATING status
                var order = new Order
                {
                    OrderType = OrderType.SWAP,
                    ListingID = dto.ListingID,
                    BuyerID = buyerId,
                    Price = null,
                    DeliveryFee = deliveryFee,
                    Status = OrderStatus.NEGOTIATING, // Starts with NEGOTIATING
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create negotiation
                var negotiation = new Negotiation
                {
                    OrderID = order.OrderID,
                    OfferedListingID = dto.OfferedListingID,
                    Message = dto.Message,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Negotiations.Add(negotiation);

                // Create delivery if address provided
                if (!string.IsNullOrWhiteSpace(dto.DropoffAddress))
                {
                    var delivery = new Delivery
                    {
                        OrderID = order.OrderID,
                        PickupAddress = requestedListing.User.Address ?? string.Empty,
                        PickupPhone = requestedListing.User.Phone ?? string.Empty,
                        DropoffAddress = dto.DropoffAddress,
                        DropoffPhone = dto.DropoffPhone ?? string.Empty,
                        DeliveryFee = deliveryFee,
                        Status = DeliveryStatus.ASSIGNED,
                        DeliveryPersonConfirmed = false,
                        BuyerConfirmed = false
                    };
                    _context.Deliveries.Add(delivery);
                }

                // Reserve quantities (pending seller acceptance)
                requestedListing.AvailableQuantity -= dto.Quantity;
                offeredListing.AvailableQuantity -= dto.Quantity;

                _context.BookListings.Update(requestedListing);
                _context.BookListings.Update(offeredListing);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Respond to swap (accept or reject)
        public async Task<bool> RespondToSwapAsync(int orderId, bool accept)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Listing)
                    .Include(o => o.Negotiations)
                    .FirstOrDefaultAsync(o => o.OrderID == orderId);

                if (order == null || order.OrderType != OrderType.SWAP)
                    return false;

                if (order.Status != OrderStatus.NEGOTIATING)
                    return false;

                if (accept)
                {
                    // Accept the swap
                    order.Status = OrderStatus.ACCEPTED;
                }
                else
                {
                    // Reject the swap - restore quantities
                    order.Status = OrderStatus.REJECTED;

                    var requestedListing = await _context.BookListings.FindAsync(order.ListingID);
                    if (requestedListing != null)
                    {
                        requestedListing.AvailableQuantity += order.Quantity;
                        _context.BookListings.Update(requestedListing);
                    }

                    // Restore offered listing quantity
                    var negotiation = order.Negotiations.FirstOrDefault();
                    if (negotiation?.OfferedListingID != null)
                    {
                        var offeredListing = await _context.BookListings.FindAsync(negotiation.OfferedListingID);
                        if (offeredListing != null)
                        {
                            offeredListing.AvailableQuantity += order.Quantity;
                            _context.BookListings.Update(offeredListing);
                        }
                    }
                }

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Helper method to map to DTO
        private static OrderDisplayDTO MapToOrderDisplayDTO(Order o)
        {
            return new OrderDisplayDTO
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
                DeliveryStatus = o.Delivery?.Status,
                PickedUpAt = o.Delivery?.PickedUpAt,
                DeliveredAt = o.Delivery?.DeliveredAt,
                DeliveryPersonName = o.Delivery?.DeliveryPerson?.Name,
                PaymentStatus = o.Payment?.Status,
                PaidAt = o.Payment?.PaidAt,
                OrderScore = o.Rating?.OrderScore,
                DeliveryScore = o.Rating?.DeliveryScore,
                RatingComment = o.Rating?.Comment
            };
        }
    }
}