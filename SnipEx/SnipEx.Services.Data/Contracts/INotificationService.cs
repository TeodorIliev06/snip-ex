namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Notification;

    public interface INotificationService
    {
        Task<IEnumerable<NotificationViewModel>> GetUserNotificationsAsync(string userId,
            int page = 1, int pageSize = 10);

        Task<int> GetUnreadNotificationsCountAsync(string userId);

        Task<int> GetTotalNotificationsCountAsync(string userId);

        Task<bool> MarkNotificationAsReadAsync(Guid notificationGuid, string userId);

        Task<bool> MarkAllNotificationsAsReadAsync(string userId);
    }
}
