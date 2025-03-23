namespace SnipEx.Data.Models
{
    using SnipEx.Data.Models.Enums;

    public class Notification
    {
        public Guid Id { get; set; }

        public string Message { get; set; } = null!;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid RecipientId { get; set; }

        public virtual ApplicationUser Recipient { get; set; } = null!;

        public Guid ActorId { get; set; }

        public virtual ApplicationUser Actor { get; set; } = null!;

        public Guid RelatedEntityId { get; set; }

        public string RelatedEntityType { get; set; } = null!;
    }
}
