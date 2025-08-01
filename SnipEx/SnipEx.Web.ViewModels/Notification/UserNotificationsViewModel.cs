namespace SnipEx.Web.ViewModels.Notification
{
    public class UserNotificationsViewModel
    {
        public IEnumerable<NotificationViewModel> Notifications { get; set; } 
            = new HashSet<NotificationViewModel>();

        public int TotalCount { get; set; }

        public bool HasMoreNotifications { get; set; }

        public int CurrentPage { get; set; } = 1;
    }
}
