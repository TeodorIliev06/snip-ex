namespace SnipEx.Web.ViewModels.Admin
{
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    public class ManageUserViewModel : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public bool IsBanned { get; set; }
    }
}
