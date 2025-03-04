namespace SnipEx.Web.ViewModels.Post
{
    using SnipEx.Web.ViewModels.Tag;

    public class PostCarouselViewModel
    {
        public IEnumerable<PostCardViewModel> FeaturedPosts { get; set; }
            = new HashSet<PostCardViewModel>();

        public IEnumerable<TagViewModel> TrendingTags { get; set; }
            = new HashSet<TagViewModel>();
    }
}
