namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;
    using SnipEx.Data.Extensions;

    using static Common.EntityValidationConstants.Comment;

    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .HasKey(c => c.Id);

            builder
                .Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(ContentMaxLength);

            builder
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

            builder
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .SeedDataFromJson("comments.json");
        }
    }
}
