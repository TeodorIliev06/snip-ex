namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Notification;

    public interface INotificationService
    {
        Task<IEnumerable<NotificationViewModel>> GetUserNotificationsAsync(string userId);
    }
}
