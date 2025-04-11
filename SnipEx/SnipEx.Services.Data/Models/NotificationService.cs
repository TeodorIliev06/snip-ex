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
    }
}
