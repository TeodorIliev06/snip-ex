namespace SnipEx.Web.ViewModels.Notification
{
    using SnipEx.Web.ViewModels.User;

    public class UserNotificationsViewModel
    {
        public IEnumerable<NotificationViewModel> Notifications { get; set; } 
            = new HashSet<NotificationViewModel>();

        public int TotalCount { get; set; }

        public int FilteredNotificationsCount { get; set; }

        public string CurrentFilter { get; set; } = "all";

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
