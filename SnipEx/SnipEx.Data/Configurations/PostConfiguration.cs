namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;
    using SnipEx.Data.Extensions;

    using static Common.EntityValidationConstants.Post;

    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder
                .HasKey(p => p.Id);

            builder
                .Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(TitleMaxLength);

            builder
                .Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(ContentMaxLength);

            builder
                .Property(p => p.Views)
                .HasDefaultValue(ViewMinValue);

            builder
                .Property(p => p.Rating)
                .HasPrecision(2, 1);

            builder.HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .SeedDataFromJson("posts.json");
        }
    }
}
