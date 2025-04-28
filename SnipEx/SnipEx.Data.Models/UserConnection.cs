namespace SnipEx.Data.Models
{
    using SnipEx.Data.Models.Enums;

    public class UserConnection
    {
        public UserConnection()
        {
            this.ConnectedOn = DateTime.UtcNow;
            this.Status = ConnectionStatus.Pending;
        }

        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public Guid ConnectedUserId { get; set; }
        public virtual ApplicationUser ConnectedUser { get; set; } = null!;

        public DateTime ConnectedOn { get; set; } 

        public ConnectionStatus Status { get; set; }
    }
}
