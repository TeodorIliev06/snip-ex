namespace SnipEx.Data.Models
{
    public class Post
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int Views { get; set; }

        public decimal Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public Guid LanguageId { get; set; }

        public ProgrammingLanguage Language { get; set; } = null!;

        public ICollection<PostTag> PostsTags { get; set; }
            = new HashSet<PostTag>();

        public ICollection<Comment> Comments { get; set; }
            = new HashSet<Comment>();

        public ICollection<PostLike> Likes { get; set; }
            = new HashSet<PostLike>();
    }
}
