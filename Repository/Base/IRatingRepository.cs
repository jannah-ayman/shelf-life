using ShelfLife.DTOs;
using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface IRatingRepository
    {
        Task AddAsync(Rating rating);
        Task<Rating?> CreateRatingAsync(CreateRatingDTO dto);
        Task<RatingDisplayDTO?> GetRatingByOrderIdAsync(int orderId);
        Task<IEnumerable<RatingDisplayDTO>> GetUserRatingsAsync(int userId);
        Task<bool> OrderHasRatingAsync(int orderId);
        Task SaveChangesAsync();

       
        Task<double> UpdateUserAverageRatingAsync(int userId);
    }
}
