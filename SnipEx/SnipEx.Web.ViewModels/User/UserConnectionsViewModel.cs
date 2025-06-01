namespace SnipEx.Web.ViewModels.User
{
    public class UserConnectionsViewModel
    {
        public IEnumerable<ConnectionViewModel> Connections { get; set; }
            = new HashSet<ConnectionViewModel>();
    }
}
