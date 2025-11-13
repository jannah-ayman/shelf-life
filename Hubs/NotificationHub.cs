using Microsoft.AspNetCore.SignalR;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Hubs
{
    public class NotificationHub : Hub
    {
        // Join user to their notification group when they connect
        public async Task JoinUserGroup(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        // Leave user from their notification group when they disconnect
        public async Task LeaveUserGroup(int userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
    }

    // Notification service to send notifications from anywhere in the application
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepo)
        {
            _hubContext = hubContext;
            _notificationRepo = notificationRepo;
        }

        // Send notification and save to database
        public async Task SendNotificationAsync(int userId, NotificationType type, string title, string? message = null)
        {
            // Save to database
            var notification = await _notificationRepo.CreateNotificationAsync(userId, type, title, message);

            if (notification != null)
            {
                // Send real-time notification via SignalR to user's group
                await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notification);
            }
        }

        // Send notification to multiple users
        public async Task SendNotificationToUsersAsync(List<int> userIds, NotificationType type, string title, string? message = null)
        {
            foreach (var userId in userIds)
            {
                await SendNotificationAsync(userId, type, title, message);
            }
        }
    }
}

