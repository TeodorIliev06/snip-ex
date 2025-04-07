namespace SnipEx.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();
        }

        public string? ProfilePicturePath { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
            = new HashSet<Post>();

        public virtual ICollection<Post> Bookmarks { get; set; }
            = new HashSet<Post>();

        public ICollection<PostLike> LikedPosts { get; set; }
            = new HashSet<PostLike>();

        public virtual ICollection<Comment> Comments { get; set; }
            = new HashSet<Comment>();

        public ICollection<CommentLike> LikedComments { get; set; }
            = new HashSet<CommentLike>();

        public virtual ICollection<Notification> Notifications { get; set; }
            = new HashSet<Notification>();
    }
}
