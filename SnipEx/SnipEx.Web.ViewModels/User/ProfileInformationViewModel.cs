namespace SnipEx.Web.ViewModels.User
{
    using System.Globalization;

    using AutoMapper;
    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Mapping.Contracts;

    using static Common.EntityValidationConstants.User;

    public class ProfileInformationViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Username { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string JoinDate { get; set; } = null!;

        public int PostsCount { get; set; }

        public int BookmarksCount { get; set; }

        public int ConnectionsCount { get; set; }

        public bool IsCurrentUser { get; set; }

        public bool IsConnected { get; set; }

        public IEnumerable<PostCardViewModel> RecentPosts { get; set; }
            = new HashSet<PostCardViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, ProfileInformationViewModel>()
                .ForMember(d => d.JoinDate,
                    opt =>
                        opt.MapFrom(c => c.JoinDate.ToString(JoinDateFormat, CultureInfo.InvariantCulture)))
                .ForMember(d => d.IsConnected, opt => opt.Ignore());
        }
    }
}
