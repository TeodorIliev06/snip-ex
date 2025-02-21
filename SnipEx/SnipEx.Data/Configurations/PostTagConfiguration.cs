namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;

    public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
    {
        public void Configure(EntityTypeBuilder<PostTag> builder)
        {
            builder
                .HasKey(pt => new { pt.PostId, pt.TagId });

            builder
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostsTags)
                .HasForeignKey(pt => pt.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostsTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
