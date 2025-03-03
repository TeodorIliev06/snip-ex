namespace SnipEx.Data.Models
{
    public class ProgrammingLanguage
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string FileExtension { get; set; } = null!; //Maybe re-facture with enum

        public ICollection<Post> Posts { get; set; }
            = new HashSet<Post>();
    }
}
