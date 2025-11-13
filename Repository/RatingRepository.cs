using Microsoft.EntityFrameworkCore;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;
using System;
using System.Linq;

namespace ShelfLife.Repository
{
    public class RatingRepository : IRatingRepository
    {
        private readonly DBcontext _context;
        private readonly IDeliveryPersonRepository _deliveryPersonRepo;

        public RatingRepository(DBcontext context, IDeliveryPersonRepository deliveryPersonRepo)
        {
            _context = context;
            _deliveryPersonRepo = deliveryPersonRepo;
        }

        public async Task<bool> OrderHasRatingAsync(int orderId)
        {
            return await _context.Ratings.AnyAsync(r => r.OrderID == orderId);
        }

        public async Task<Rating?> CreateRatingAsync(CreateRatingDTO dto)
        {
            var order = await _context.Orders
                .Include(o => o.Listing)
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(o => o.OrderID == dto.OrderID);

            if (order == null || order.Status != OrderStatus.COMPLETED)
                return null;

            if (await OrderHasRatingAsync(order.OrderID))
                return null;

            var rating = new Rating
            {
                OrderID = dto.OrderID,
                OrderScore = dto.OrderScore,
                DeliveryScore = dto.DeliveryScore,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            // Update average rating for the seller (User)
            await UpdateUserAverageRatingAsync(order.Listing.UserID);

            // Update average rating for the delivery person if delivery exists
            if (order.Delivery != null && order.Delivery.DeliveryPersonID.HasValue)
            {
                await _deliveryPersonRepo.UpdateAverageRatingAsync(order.Delivery.DeliveryPersonID.Value);
            }

            return rating;
        }

        public async Task<RatingDisplayDTO?> GetRatingByOrderIdAsync(int orderId)
        {
            return await _context.Ratings
                .Where(r => r.OrderID == orderId)
                .Select(r => new RatingDisplayDTO
                {
                    RatingID = r.RatingID,
                    OrderID = r.OrderID,
                    OrderScore = r.OrderScore,
                    DeliveryScore = r.DeliveryScore,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RatingDisplayDTO>> GetUserRatingsAsync(int userId)
        {
            return await _context.Ratings
                .Include(r => r.Order)
                .ThenInclude(o => o.Listing)
                .Where(r => r.Order.Listing.UserID == userId)
                .Select(r => new RatingDisplayDTO
                {
                    RatingID = r.RatingID,
                    OrderID = r.OrderID,
                    OrderScore = r.OrderScore,
                    DeliveryScore = r.DeliveryScore,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<double> UpdateUserAverageRatingAsync(int userId)
        {
            var ratings = await _context.Ratings
                .Include(r => r.Order)
                .ThenInclude(o => o.Listing)
                .Where(r => r.Order.Listing.UserID == userId)
                .ToListAsync();

            double avg = 0.0;
            
            if (ratings.Any())
            {
                avg = ratings.Average(r => (r.OrderScore + r.DeliveryScore) / 2.0);
            }

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.AverageRating = (decimal)avg;
                await _context.SaveChangesAsync();
            }

            return avg;
        }

        public async Task AddAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
