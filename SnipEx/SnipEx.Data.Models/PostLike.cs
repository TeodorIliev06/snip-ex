namespace SnipEx.Data.Models
{
    public class PostLike
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public virtual Post Post { get; set; } = null!;

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
