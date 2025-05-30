namespace SnipEx.Web.ViewModels.DTOs
{
    public class NotificationDto
    {
        public string Message { get; set; } = null!;
        public string RecipientId { get; set; } = null!;
        public string ActorId { get; set; } = null!;
    }
}
