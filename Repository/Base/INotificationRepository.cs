using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface INotificationRepository
    {
        Task<Notification?> CreateNotificationAsync(int userId, NotificationType type, string title, string? message = null);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId);
    }
}

