namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Data.Models.Enums;
    using SnipEx.Web.ViewModels.Notification;

    public interface INotificationService
    {
        Task<IEnumerable<NotificationViewModel>> GetUserNotificationsAsync(string userId,
            int page = 1, int pageSize = 4);

        Task<IEnumerable<NotificationViewModel>> GetUserNotificationsByTypesAsync(string userId,
            IEnumerable<NotificationType> notificationTypes, int skip = 0, int take = 10);

        Task<int> GetNotificationsCountByTypesAsync(string userId,
            IEnumerable<NotificationType> notificationTypes);

        Task<int> GetUnreadNotificationsCountAsync(string userId);

        Task<int> GetTotalNotificationsCountAsync(string userId);

        Task<bool> MarkNotificationAsReadAsync(Guid notificationGuid, string userId);

        Task<bool> MarkAllNotificationsAsReadAsync(string userId);
    }
}
