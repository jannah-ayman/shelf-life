using Microsoft.EntityFrameworkCore;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DBcontext _context;

        public UserRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.BookListings)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserID == userId)
                .Select(u => new UserProfileDTO
                {
                    UserID = u.UserID,
                    UserType = u.UserType,
                    Email = u.Email,
                    Phone = u.Phone,
                    Name = u.Name,
                    Address = u.Address,
                    City = u.City,
                    ProfilePhotoURL = u.ProfilePhotoURL,
                    AverageRating = u.AverageRating,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    TotalListings = u.BookListings.Count,
                    TotalOrders = u.Orders.Count
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Soft delete by marking inactive
            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPasswordHash, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PasswordHash != currentPasswordHash)
                return false;

            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDashboardStatsDTO> GetDashboardStatsAsync(int userId)
        {
            var listings = await _context.BookListings
                .Where(b => b.UserID == userId)
                .ToListAsync();

            var incomingOrders = await _context.Orders
                .Include(o => o.Listing)
                .Where(o => o.Listing.UserID == userId)
                .ToListAsync();

            var outgoingOrders = await _context.Orders
                .Where(o => o.BuyerID == userId)
                .ToListAsync();

            var user = await _context.Users.FindAsync(userId);

            return new UserDashboardStatsDTO
            {
                TotalListings = listings.Count,
                ActiveListings = listings.Count(l => l.AvailableQuantity > 0),
                SoldItems = listings.Count(l => l.AvailabilityStatus == AvailabilityStatus.Sold),
                SwappedItems = listings.Count(l => l.AvailabilityStatus == AvailabilityStatus.Swapped),
                IncomingOrders = incomingOrders.Count,
                OutgoingOrders = outgoingOrders.Count,
                PendingOrders = incomingOrders.Count(o => o.Status == OrderStatus.NEGOTIATING),
                CompletedOrders = incomingOrders.Count(o => o.Status == OrderStatus.COMPLETED),
                TotalEarnings = incomingOrders
                    .Where(o => o.Status == OrderStatus.COMPLETED && o.OrderType == OrderType.SALE)
                    .Sum(o => o.Price ?? 0),
                AverageRating = user?.AverageRating ?? 0
            };
        }
    }
}