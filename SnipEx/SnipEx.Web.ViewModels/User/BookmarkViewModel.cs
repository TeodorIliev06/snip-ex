namespace SnipEx.Web.ViewModels.User
{
    using AutoMapper;

    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Mapping.Contracts;


    public class BookmarkViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        private const int RecentDays = -30;

        public int TotalBookmarks { get; set; }

        public int RecentBookmarksCount { get; set; }

        public string MostCommonLanguage { get; set; } = "None";

        public IEnumerable<PostCardViewModel> Bookmarks { get; set; }
            = new HashSet<PostCardViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, BookmarkViewModel>()
                .ForMember(d => d.TotalBookmarks,
                    opt => opt.MapFrom(s => s.Bookmarks.Count))
                .ForMember(d => d.RecentBookmarksCount,
                    opt => opt.MapFrom(s => s.Bookmarks
                        .Count(b => b.CreatedAt >= DateTime.UtcNow.AddDays(RecentDays))))
                .ForMember(d => d.MostCommonLanguage,
                    opt => opt.MapFrom(s => s.Bookmarks
                        .GroupBy(b => b.Language.Name)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault() ?? "None"));
        }
    }
}
