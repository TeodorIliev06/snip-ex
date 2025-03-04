namespace SnipEx.Web.ViewModels.Post
{
    public class PostCarouselViewModel
    {
        public IEnumerable<PostCardViewModel> FeaturedPosts { get; set; }
            = new HashSet<PostCardViewModel>();
    }
}
