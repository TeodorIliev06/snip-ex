namespace SnipEx.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();
            this.JoinDate = DateTime.UtcNow;
        }

        public string? ProfilePicturePath { get; set; }

        public DateTime JoinDate { get; set; }

        public bool IsBanned { get; set; }

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

        public ICollection<UserConnection> Connections { get; set; }
            = new HashSet<UserConnection>();

    }
}
