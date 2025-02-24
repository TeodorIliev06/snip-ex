namespace SnipEx.Web.ViewModels.Post
{
    using SnipEx.Web.ViewModels.Tag;

    public class PostIndexViewModel
    {
        public ICollection<PostViewModel> Posts { get; set; }
            = new List<PostViewModel>();

        public ICollection<TagViewModel> PopularTags { get; set; }
            = new List<TagViewModel>();

        public string? SearchQuery { get; set; }

        public string? CurrentSort { get; set; }

        public string? SelectedTag { get; set; }
    }
}
