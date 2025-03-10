namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;

    public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
    {
        public void Configure(EntityTypeBuilder<CommentLike> builder)
        {
            builder
                .HasKey(cl => cl.Id);

            builder
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(cl => cl.User)
                .WithMany(u => u.LikedComments)
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent duplicating in likes
            builder
                .HasIndex(cl => new { cl.CommentId, cl.UserId })
                .IsUnique();
        }
    }
}
