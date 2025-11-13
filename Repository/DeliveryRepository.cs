using Microsoft.EntityFrameworkCore;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly DBcontext _context;
        private const decimal PLATFORM_FEE_PERCENTAGE = 0.20m; // 20% platform fee for delivery

        public DeliveryRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task<bool> HasActiveDeliveryAsync(int deliveryPersonId)
        {
            return await _context.Deliveries
                .AnyAsync(d => d.DeliveryPersonID == deliveryPersonId &&
                             (d.Status == DeliveryStatus.ASSIGNED ||
                              d.Status == DeliveryStatus.PICKED_UP));
        }

        public async Task<IEnumerable<DeliveryOrderDisplayDTO>> GetAvailableOrdersForDeliveryAsync(DeliveryOrderFilterDTO? filter = null)
        {
            var query = _context.Orders
            .Include(o => o.Listing)
            .ThenInclude(l => l.User)
            .Include(o => o.Buyer)
            .Where(o => o.Status == OrderStatus.ACCEPTED &&
            (o.OrderType == OrderType.SALE || o.OrderType == OrderType.SWAP) &&
            !_context.Deliveries.Any(d => d.OrderID == o.OrderID &&
                                    (d.Status == DeliveryStatus.ASSIGNED ||
                                    d.Status == DeliveryStatus.PICKED_UP)))
                                    .AsQueryable();
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.City))
                {
                    query = query.Where(o => o.Listing.City == filter.City ||
                                            o.Buyer.City == filter.City);
                }

                if (filter.OrderType.HasValue)
                {
                    query = query.Where(o => o.OrderType == filter.OrderType.Value);
                }
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new DeliveryOrderDisplayDTO
                {
                    OrderID = o.OrderID,
                    OrderType = o.OrderType,
                    BookTitle = o.Listing.Title,
                    BookAuthor = o.Listing.Author,
                    BookPhotoURL = o.Listing.PhotoURLs,
                    PickupAddress = o.Listing.User.Address ?? "",
                    PickupPhone = o.Listing.User.Phone ?? "",
                    PickupCity = o.Listing.City,
                    DropoffAddress = o.Buyer.Address ?? "",
                    DropoffPhone = o.Buyer.Phone ?? "",
                    DropoffCity = o.Buyer.City,
                    DeliveryFee = o.DeliveryFee ?? CalculateDeliveryFee(o.Listing.City, o.Buyer.City),
                    EstimatedDistance = null,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            // Apply fee filtering in memory
            if (filter?.MinFee.HasValue == true)
            {
                orders = orders.Where(o => o.DeliveryFee >= filter.MinFee.Value).ToList();
            }
            if (filter?.MaxFee.HasValue == true)
            {
                orders = orders.Where(o => o.DeliveryFee <= filter.MaxFee.Value).ToList();
            }

            return orders;
        }

        public async Task<DeliveryDetailDTO?> GetActiveDeliveryByPersonAsync(int deliveryPersonId)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Listing)
                        .ThenInclude(l => l.User)
                .Include(d => d.Order.Buyer)
                .Where(d => d.DeliveryPersonID == deliveryPersonId &&
                           (d.Status == DeliveryStatus.ASSIGNED ||
                            d.Status == DeliveryStatus.PICKED_UP))
                .Select(d => MapToDeliveryDetailDTO(d))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DeliveryDetailDTO>> GetDeliveryHistoryAsync(int deliveryPersonId)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Listing)
                        .ThenInclude(l => l.User)
                .Include(d => d.Order.Buyer)
                .Where(d => d.DeliveryPersonID == deliveryPersonId)
                .OrderByDescending(d => d.PickedUpAt ?? d.Order.CreatedAt)
                .Select(d => MapToDeliveryDetailDTO(d))
                .ToListAsync();
        }

        public async Task<DeliveryDetailDTO?> GetDeliveryByIdAsync(int deliveryId)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Listing)
                        .ThenInclude(l => l.User)
                .Include(d => d.Order.Buyer)
                .Where(d => d.DeliveryID == deliveryId)
                .Select(d => MapToDeliveryDetailDTO(d))
                .FirstOrDefaultAsync();
        }

        public async Task<DeliveryDetailDTO?> CreateDeliveryForOrderAsync(int orderId, int deliveryPersonId)
        {
            var order = await _context.Orders
                .Include(o => o.Listing)
                    .ThenInclude(l => l.User)
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null || order.Status != OrderStatus.ACCEPTED)
                return null;

            var deliveryFee = CalculateDeliveryFee(order.Listing.City, order.Buyer.City);

            if (!order.DeliveryFee.HasValue)
            {
                order.DeliveryFee = deliveryFee;
            }

            var delivery = new Delivery
            {
                OrderID = orderId,
                DeliveryPersonID = deliveryPersonId,
                PickupAddress = order.Listing.User.Address ?? "",
                PickupPhone = order.Listing.User.Phone ?? "",
                DropoffAddress = order.Buyer.Address ?? "",
                DropoffPhone = order.Buyer.Phone ?? "",
                DeliveryFee = order.DeliveryFee.Value,
                Status = DeliveryStatus.ASSIGNED
            };

            _context.Deliveries.Add(delivery);
            order.Status = OrderStatus.DELIVERY_ASSIGNED;

            await _context.SaveChangesAsync();

            return await GetDeliveryByIdAsync(delivery.DeliveryID);
        }

        public async Task<DeliveryDetailDTO?> UpdateDeliveryStatusAsync(int deliveryId, DeliveryStatus newStatus)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.DeliveryID == deliveryId);

            if (delivery == null)
                return null;

            delivery.Status = newStatus;

            if (newStatus == DeliveryStatus.PICKED_UP)
            {
                delivery.PickedUpAt = DateTime.UtcNow;
                delivery.Order.Status = OrderStatus.DELIVERING;
            }

            await _context.SaveChangesAsync();

            return await GetDeliveryByIdAsync(deliveryId);
        }

        public async Task<DeliveryDetailDTO?> StartDeliveringAsync(int deliveryId)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.DeliveryID == deliveryId);

            if (delivery == null || delivery.Status != DeliveryStatus.PICKED_UP)
                return null;

            delivery.Order.Status = OrderStatus.DELIVERING;

            await _context.SaveChangesAsync();

            return await GetDeliveryByIdAsync(deliveryId);
        }

        public async Task<DeliveryDetailDTO?> ConfirmDeliveryByPersonAsync(int deliveryId)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Listing)
                .Include(d => d.Order.Negotiations)
                .Include(d => d.DeliveryPerson)
                .FirstOrDefaultAsync(d => d.DeliveryID == deliveryId);

            if (delivery == null)
                return null;

            if (delivery.Status != DeliveryStatus.PICKED_UP || delivery.Order.Status != OrderStatus.DELIVERING)
                return null;

            delivery.DeliveryPersonConfirmed = true;
            delivery.PickedUpAt ??= DateTime.UtcNow;

            // Only mark as fully delivered if buyer has also confirmed
            if (delivery.BuyerConfirmed)
            {
                await CompleteDeliveryAsync(delivery);
            }

            await _context.SaveChangesAsync();
            return await GetDeliveryByIdAsync(deliveryId);
        }

        public async Task<DeliveryDetailDTO?> ConfirmDeliveryByBuyerAsync(int deliveryId)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Listing)
                .Include(d => d.Order.Negotiations)
                .Include(d => d.DeliveryPerson)
                .FirstOrDefaultAsync(d => d.DeliveryID == deliveryId);

            if (delivery == null)
                return null;

            if (delivery.Status != DeliveryStatus.PICKED_UP || delivery.Order.Status != OrderStatus.DELIVERING)
                return null;

            delivery.BuyerConfirmed = true;

            // Only mark as fully delivered if delivery person already confirmed
            if (delivery.DeliveryPersonConfirmed)
            {
                await CompleteDeliveryAsync(delivery);
            }

            await _context.SaveChangesAsync();
            return await GetDeliveryByIdAsync(deliveryId);
        }

        private async Task CompleteDeliveryAsync(Delivery delivery)
        {
            delivery.Status = DeliveryStatus.DELIVERED;
            delivery.Order.Status = OrderStatus.COMPLETED;
            delivery.Order.CompletedAt = DateTime.UtcNow;
            delivery.DeliveredAt = DateTime.UtcNow;

            // Update listing status if quantity reaches 0
            var listing = delivery.Order.Listing;
            if (listing.AvailableQuantity == 0)
            {
                listing.AvailabilityStatus = delivery.Order.OrderType == OrderType.SALE
                    ? AvailabilityStatus.Sold
                    : AvailabilityStatus.Swapped;
                _context.BookListings.Update(listing);
            }

            // For swaps, also update the offered listing
            if (delivery.Order.OrderType == OrderType.SWAP)
            {
                var negotiation = delivery.Order.Negotiations.FirstOrDefault();
                if (negotiation?.OfferedListingID != null)
                {
                    var offeredListing = await _context.BookListings.FindAsync(negotiation.OfferedListingID);
                    if (offeredListing != null && offeredListing.AvailableQuantity == 0)
                    {
                        offeredListing.AvailabilityStatus = AvailabilityStatus.Swapped;
                        _context.BookListings.Update(offeredListing);
                    }
                }
            }

            // Process payment
            await ProcessDeliveryPaymentAsync(delivery);
        }

        public async Task<bool> CancelDeliveryAsync(int deliveryId)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.DeliveryID == deliveryId);

            if (delivery == null)
                return false;

            // Set order back to ACCEPTED
            delivery.Order.Status = OrderStatus.ACCEPTED;

            // Remove delivery entirely so a new delivery can be assigned
            _context.Deliveries.Remove(delivery);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DeliveryPersonStatsDTO> GetDeliveryPersonStatsAsync(int deliveryPersonId)
        {
            var deliveryPerson = await _context.DeliveryPeople
                .Include(dp => dp.Deliveries)
                .FirstOrDefaultAsync(dp => dp.DeliveryPersonID == deliveryPersonId);

            if (deliveryPerson == null)
            {
                return new DeliveryPersonStatsDTO();
            }

            var deliveries = deliveryPerson.Deliveries;

            return new DeliveryPersonStatsDTO
            {
                TotalDeliveries = deliveries.Count,
                CompletedDeliveries = deliveries.Count(d => d.Status == DeliveryStatus.DELIVERED),
                ActiveDeliveries = deliveries.Count(d => d.Status == DeliveryStatus.ASSIGNED ||
                                                         d.Status == DeliveryStatus.PICKED_UP),
                CancelledDeliveries = deliveries.Count(d => d.Status == DeliveryStatus.FAILED),
                TotalEarnings = deliveryPerson.TotalEarnings,
                AverageRating = deliveryPerson.AverageRating,
                TotalRatings = deliveryPerson.TotalDeliveries
            };
        }

        public async Task<int> CleanupStaleDeliveriesAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

            var staleDeliveries = await _context.Deliveries
                .Include(d => d.Order)
                .Where(d => d.Status == DeliveryStatus.ASSIGNED &&
                           d.Order.CreatedAt < oneWeekAgo)
                .ToListAsync();

            foreach (var delivery in staleDeliveries)
            {
                delivery.Order.Status = OrderStatus.ACCEPTED;
                delivery.Status = DeliveryStatus.FAILED;
                delivery.DeliveryPersonID = null;
            }

            await _context.SaveChangesAsync();
            return staleDeliveries.Count;
        }

        // Helper methods

        private static decimal CalculateDeliveryFee(string? pickupCity, string? dropoffCity)
        {
            if (string.IsNullOrEmpty(pickupCity) || string.IsNullOrEmpty(dropoffCity))
                return 50m;

            if (pickupCity.Equals(dropoffCity, StringComparison.OrdinalIgnoreCase))
                return 30m;

            return 80m;
        }

        private async Task ProcessDeliveryPaymentAsync(Delivery delivery)
        {
            var order = delivery.Order;
            var deliveryPerson = delivery.DeliveryPerson;

            if (deliveryPerson == null)
                return;

            // Calculate delivery person earnings: 80% for both SALE and SWAP
            decimal deliveryPersonEarnings = delivery.DeliveryFee * (1 - PLATFORM_FEE_PERCENTAGE);

            // Update delivery person earnings
            deliveryPerson.TotalEarnings += deliveryPersonEarnings;
            deliveryPerson.TotalDeliveries += 1;

            // Update payment status
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == order.OrderID);
            if (payment != null)
            {
                payment.Status = PaymentStatus.COMPLETED;
                payment.PaidAt = DateTime.UtcNow;
                _context.Payments.Update(payment);
            }

            await _context.SaveChangesAsync();
        }

        private static DeliveryDetailDTO MapToDeliveryDetailDTO(Delivery d)
        {
            return new DeliveryDetailDTO
            {
                DeliveryID = d.DeliveryID,
                OrderID = d.OrderID,
                DeliveryPersonID = d.DeliveryPersonID ?? 0,
                OrderType = d.Order.OrderType,
                Status = d.Status,
                BookTitle = d.Order.Listing.Title,
                BookAuthor = d.Order.Listing.Author,
                BookPhotoURL = d.Order.Listing.PhotoURLs,
                Quantity = d.Order.Quantity,
                PickupAddress = d.PickupAddress,
                PickupPhone = d.PickupPhone,
                PickupCity = d.Order.Listing.City,
                SellerID = d.Order.Listing.UserID,
                SellerName = d.Order.Listing.User.Name,
                DropoffAddress = d.DropoffAddress,
                DropoffPhone = d.DropoffPhone,
                DropoffCity = d.Order.Buyer.City,
                BuyerID = d.Order.BuyerID,
                BuyerName = d.Order.Buyer.Name,
                DeliveryFee = d.DeliveryFee,
                EstimatedDistance = null,
                DeliveryPersonConfirmed = d.DeliveryPersonConfirmed,
                BuyerConfirmed = d.BuyerConfirmed,
                PickedUpAt = d.PickedUpAt,
                DeliveredAt = d.DeliveredAt,
                CreatedAt = d.Order.CreatedAt
            };
        }
    }
}