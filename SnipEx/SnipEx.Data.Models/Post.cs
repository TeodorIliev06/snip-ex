namespace SnipEx.Data.Models
{
    public class Post
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int Views { get; set; }

        public decimal Rating { get; set; }
    }
}
