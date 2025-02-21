namespace SnipEx.Data.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid PostId { get; set; }

        public virtual Post Post { get; set; } = null!;

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
