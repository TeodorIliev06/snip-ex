namespace SnipEx.Web.ViewModels.User
{
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping.Contracts;

    public class ProfileInformationViewModel : IMapFrom<ApplicationUser>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int SnippetsCount { get; set; }
    }
}
