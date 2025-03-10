namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;

    public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
    {
        public void Configure(EntityTypeBuilder<PostLike> builder)
        {
            builder
                .HasKey(pl => pl.Id);

            builder
                .HasOne(pl => pl.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(pl => pl.User)
                .WithMany(u => u.LikedPosts)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent duplicating in likes
            builder
                .HasIndex(pl => new { pl.PostId, pl.UserId })
                .IsUnique();
        }
    }
}
