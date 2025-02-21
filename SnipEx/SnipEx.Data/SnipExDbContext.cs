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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
