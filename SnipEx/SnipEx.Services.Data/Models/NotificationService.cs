namespace SnipEx.Services.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Notification;

    public class NotificationService(
        IRepository<Notification, Guid> notificationRepository) : INotificationService
    {
        public async Task<IEnumerable<NotificationViewModel>> GetUserNotificationsAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            var viewModel = await notificationRepository
                .GetAllAttached()
                .Include(n => n.Recipient)
                .Include(n => n.Actor)
                .Where(n => n.RecipientId == userGuid)
                .To<NotificationViewModel>()
                .ToListAsync();

            return viewModel;
        }

        public async Task<int> GetUnreadNotificationsCountAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            return await notificationRepository
                .GetAllAttached()
                .Where(n => n.RecipientId == userGuid && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);
            var notification = await notificationRepository
                .FirstOrDefaultAsync(n =>
                    n.Id == notificationGuid &&
                    n.RecipientId == userGuid);

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            await notificationRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var unreadNotifications = await notificationRepository
                .GetAllAttached()
                .Where(n => n.RecipientId == userGuid && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
            {
                return false;
            }

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await notificationRepository.SaveChangesAsync();
            return true;
        }
    }
}
