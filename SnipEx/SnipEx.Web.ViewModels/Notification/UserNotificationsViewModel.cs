namespace SnipEx.Web.ViewModels.Notification
{
    public class UserNotificationsViewModel
    {
        public IEnumerable<NotificationViewModel> Notifications { get; set; } 
            = new HashSet<NotificationViewModel>();
    }
}
