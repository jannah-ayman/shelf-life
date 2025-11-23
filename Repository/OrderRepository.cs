using Microsoft.EntityFrameworkCore;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DBcontext _context;
        private const decimal NORMAL_USER_PLATFORM_FEE = 0.05m; // 5%
        private const decimal BUSINESS_USER_PLATFORM_FEE = 0.10m; // 10%

        public OrderRepository(DBcontext context)
        {
            _context = context;
        }

        // Payment breakdown calculation
        public async Task<PaymentBreakdownDTO?> GetPaymentBreakdownAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User)
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
                return null;

            var breakdown = new PaymentBreakdownDTO
            {
                OrderID = orderId,
                OrderType = order.OrderType,
                SellerUserType = order.Listing.User.UserType,
                Quantity = order.Quantity
            };

            if (order.OrderType == OrderType.SALE)
            {
                // SALE order calculations
                decimal itemPrice = order.Price ?? 0;
                decimal platformFeePercentage = order.Listing.User.UserType == UserType.NORMAL_USER
                    ? NORMAL_USER_PLATFORM_FEE
                    : BUSINESS_USER_PLATFORM_FEE;

                breakdown.ItemPrice = itemPrice / order.Quantity;
                breakdown.SubTotal = itemPrice;
                breakdown.PlatformFeePercentage = platformFeePercentage * 100;
                breakdown.PlatformFee = itemPrice * platformFeePercentage;
                breakdown.DeliveryFee = order.DeliveryFee ?? 0;
                breakdown.TotalAmount = itemPrice + breakdown.DeliveryFee;
                breakdown.BuyerPays = breakdown.TotalAmount;
                breakdown.SellerReceives = itemPrice - breakdown.PlatformFee;
                breakdown.DeliveryPersonReceives = breakdown.DeliveryFee * 0.80m; // 80% to delivery person
                breakdown.PlatformReceives = breakdown.PlatformFee + (breakdown.DeliveryFee * 0.20m);

                breakdown.PaymentSummary = $"Buyer pays: {breakdown.BuyerPays:C} " +
                    $"(Item: {breakdown.SubTotal:C} + Delivery: {breakdown.DeliveryFee:C}). " +
                    $"Seller receives: {breakdown.SellerReceives:C} after {breakdown.PlatformFeePercentage}% platform fee.";
            }
            else // SWAP
            {
                // SWAP order calculations
                breakdown.ItemPrice = 0;
                breakdown.SubTotal = 0;
                breakdown.PlatformFeePercentage = 0;
                breakdown.PlatformFee = 0;
                breakdown.DeliveryFee = order.DeliveryFee ?? 0;
                breakdown.BuyerDeliveryShare = breakdown.DeliveryFee / 2;
                breakdown.SellerDeliveryShare = breakdown.DeliveryFee / 2;
                breakdown.TotalAmount = breakdown.DeliveryFee;
                breakdown.BuyerPays = breakdown.BuyerDeliveryShare.Value;
                breakdown.SellerReceives = 0; // No money for seller in swap
                breakdown.DeliveryPersonReceives = breakdown.DeliveryFee * 0.80m; // 80% to delivery person
                breakdown.PlatformReceives = breakdown.DeliveryFee * 0.20m; // 20% to platform

                breakdown.PaymentSummary = $"Both parties split delivery fee: {breakdown.DeliveryFee:C}. " +
                    $"Each pays: {breakdown.BuyerDeliveryShare:C}. " +
                    $"Delivery person receives: {breakdown.DeliveryPersonReceives:C}.";
            }

            return breakdown;
        }

        // Existing retrieval methods
        public async Task<IEnumerable<OrderDisplayDTO>> GetUserIncomingOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User!)
                .Include(o => o.Buyer)
                .Include(o => o.Delivery!)
                    .ThenInclude(d => d.DeliveryPerson)
                .Include(o => o.Payment)
                .Include(o => o.Rating)
                .Where(o => o.Listing != null && o.Listing.UserID == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => MapToOrderDisplayDTO(o))
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDisplayDTO>> GetUserOutgoingOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User!)
                .Include(o => o.Buyer)
                .Include(o => o.Delivery!)
                    .ThenInclude(d => d.DeliveryPerson)
                .Include(o => o.Payment)
                .Include(o => o.Rating)
                .Where(o => o.Listing != null && o.BuyerID == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => MapToOrderDisplayDTO(o))
                .ToListAsync();
        }

        public async Task<OrderDisplayDTO?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User!)
                .Include(o => o.Buyer)
                .Include(o => o.Delivery!)
                    .ThenInclude(d => d.DeliveryPerson)
                .Include(o => o.Payment)
                .Include(o => o.Rating)
                .Where(o => o.OrderID == orderId && o.Listing != null)
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
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // Sale order creation - Auto-accepted with correct platform fee
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

                // ⭐ IMPORTANT: Check if there's enough quantity
                if (listing.AvailableQuantity < dto.Quantity)
                    return null;

                // Calculate fees based on seller type
                decimal basePrice = listing.Price.Value * dto.Quantity;
                decimal platformFeePercentage = listing.User.UserType == UserType.NORMAL_USER
                    ? NORMAL_USER_PLATFORM_FEE
                    : BUSINESS_USER_PLATFORM_FEE;
                decimal platformFee = basePrice * platformFeePercentage;
                decimal deliveryFee = !string.IsNullOrWhiteSpace(dto.DropoffAddress) ? 50.00m : 0m;
                decimal totalPrice = basePrice + deliveryFee;

                // Create order with ACCEPTED status (auto-accepted for sales)
                var order = new Order
                {
                    OrderType = OrderType.SALE,
                    ListingID = dto.ListingID,
                    BuyerID = buyerId,
                    Price = basePrice,
                    DeliveryFee = deliveryFee,
                    Status = OrderStatus.ACCEPTED,
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

                // ⭐ Reserve quantity and update availability status
                listing.AvailableQuantity -= dto.Quantity;

                // ⭐ If all items are sold, mark as Sold
                if (listing.AvailableQuantity <= 0)
                {
                    listing.AvailabilityStatus = AvailabilityStatus.Sold;
                }

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

                // Calculate delivery fee (split 50/50 between parties)
                decimal deliveryFee = !string.IsNullOrWhiteSpace(dto.DropoffAddress) ? 50.00m : 0m;

                // Create order with NEGOTIATING status
                var order = new Order
                {
                    OrderType = OrderType.SWAP,
                    ListingID = dto.ListingID,
                    BuyerID = buyerId,
                    Price = null,
                    DeliveryFee = deliveryFee,
                    Status = OrderStatus.NEGOTIATING,
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

                    // Create payment record for tracking
                    var payment = new Payment
                    {
                        OrderID = order.OrderID,
                        Amount = order.DeliveryFee ?? 0,
                        Status = PaymentStatus.PENDING,
                        PaidAt = DateTime.UtcNow
                    };
                    _context.Payments.Add(payment);
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

        // ⭐ NEW METHOD 1: Mark order as delivering (seller action)
        public async Task<bool> MarkAsDeliveringAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            // Only ACCEPTED orders can move to DELIVERING
            if (order.Status != OrderStatus.ACCEPTED)
                return false;

            order.Status = OrderStatus.DELIVERING;
            await _context.SaveChangesAsync();
            return true;
        }

        // ⭐ NEW METHOD 2: Seller confirms delivery
        public async Task<bool> ConfirmDeliverySellerAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            // Order must be in DELIVERING status
            if (order.Status != OrderStatus.DELIVERING)
                return false;

            order.SellerConfirmed = true;

            // If both parties confirmed, mark as completed
            if (order.BuyerConfirmed && order.SellerConfirmed)
            {
                order.Status = OrderStatus.COMPLETED;
                order.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // ⭐ NEW METHOD 3: Buyer confirms delivery
        public async Task<bool> ConfirmDeliveryBuyerAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            // Order must be in DELIVERING status
            if (order.Status != OrderStatus.DELIVERING)
                return false;

            order.BuyerConfirmed = true;

            // If both parties confirmed, mark as completed
            if (order.BuyerConfirmed && order.SellerConfirmed)
            {
                order.Status = OrderStatus.COMPLETED;
                order.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Helper method to map to DTO
        private static OrderDisplayDTO MapToOrderDisplayDTO(Order o)
        {
            var listing = o.Listing;
            var buyer = o.Buyer;
            var seller = listing?.User;

            return new OrderDisplayDTO
            {
                OrderID = o.OrderID,
                OrderType = o.OrderType,
                Status = o.Status,
                ListingID = o.ListingID,
                BookTitle = listing?.Title ?? string.Empty,
                BookAuthor = listing?.Author,
                BookCondition = listing?.Condition ?? BookCondition.GOOD,
                BookPhotoURL = listing?.PhotoURLs,
                SellerID = seller?.UserID ?? listing?.UserID ?? 0,
                SellerName = seller?.Name,
                SellerRating = seller?.AverageRating ?? 0,
                BuyerID = o.BuyerID,
                BuyerName = buyer?.Name,
                BuyerRating = buyer?.AverageRating ?? 0,
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
                RatingComment = o.Rating?.Comment,

                BuyerConfirmed = o.BuyerConfirmed,
                SellerConfirmed = o.SellerConfirmed
            };
        }
    }
}