namespace SnipEx.Data.Models
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<PostTag> PostsTags { get; set; }
            = new HashSet<PostTag>();
    }
}
