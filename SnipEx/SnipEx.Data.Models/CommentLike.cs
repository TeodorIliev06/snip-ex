namespace SnipEx.Data.Models
{
    public class CommentLike
    {
        public Guid Id { get; set; }

        public Guid CommentId { get; set; }

        public virtual Comment Comment { get; set; } = null!;

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
