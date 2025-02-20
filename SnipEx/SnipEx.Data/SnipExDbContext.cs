namespace SnipEx.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using SnipEx.Data.Models;

    public class SnipExDbContext : IdentityDbContext
    {
        public SnipExDbContext()
        {
            
        }

        public SnipExDbContext(DbContextOptions<SnipExDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Post> Posts { get; set; } = null!;
    }
}
