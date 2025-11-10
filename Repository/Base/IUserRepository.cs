using ShelfLife.DTOs;
using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<UserProfileDTO?> GetUserProfileAsync(int userId);
        Task<User?> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string currentPasswordHash, string newPasswordHash);
        Task<UserDashboardStatsDTO> GetDashboardStatsAsync(int userId);
    }
}
