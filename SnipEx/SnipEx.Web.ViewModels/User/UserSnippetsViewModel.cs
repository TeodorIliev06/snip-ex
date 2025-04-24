namespace SnipEx.Web.ViewModels.User
{
    using SnipEx.Web.ViewModels.DTOs;
    using SnipEx.Web.ViewModels.Post;

    public class UserSnippetsViewModel
    {
        public int TotalSnippets { get; set; }

        public int TotalComments { get; set; }

        public string MostPopularLanguage { get; set; } = string.Empty;

        public IEnumerable<PostCardViewModel> UserSnippets { get; set; }
            = new HashSet<PostCardViewModel>();

        public IEnumerable<string> AvailableLanguages { get; set; }
            = new HashSet<string>();

        public IEnumerable<LanguageDistributionDto> LanguageDistribution { get; set; }
            = new HashSet<LanguageDistributionDto>();
    }
}
