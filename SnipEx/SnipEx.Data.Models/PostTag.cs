namespace SnipEx.Data.Models
{
    public class PostTag
    {
        public Guid PostId { get; set; }

        public virtual Post Post { get; set; } = null!;

        public Guid TagId { get; set; }

        public virtual Tag Tag { get; set; } = null!;
    }
}
