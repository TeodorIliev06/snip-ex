namespace SnipEx.Web.ViewModels.Post
{
    using SnipEx.Web.ViewModels.Tag;

    public class PostViewModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? UserName { get; set; }

        public string CreatedAt { get; set; } = null!;

        public int Views { get; set; }

        public decimal Rating { get; set; }

        public int CommentCount { get; set; }

        public ICollection<TagViewModel> Tags { get; set; } 
            = new List<TagViewModel>();
    }
}
