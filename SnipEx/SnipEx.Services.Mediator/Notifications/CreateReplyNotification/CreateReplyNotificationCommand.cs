namespace SnipEx.Services.Mediator.Notifications.CreateReplyNotification
{
    using MediatR;

    public record CreateReplyNotificationCommand(Guid RecipientId, Guid ActorId,
            Guid RelatedEntityId, string Message, bool IsMention) : IRequest;
}
