namespace SnipEx.Web.ViewModels.Post
{
    public class PostDetailsViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LanguageName { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }

    public class CommentViewModel
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
