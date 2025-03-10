namespace SnipEx.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using System.Reflection;

    public class SnipExDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public SnipExDbContext()
        {
            
        }

        public SnipExDbContext(DbContextOptions<SnipExDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<PostLike> PostsLikes { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<PostTag> PostsTags { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<CommentLike> CommentsLikes { get; set; } = null!;
        public virtual DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
