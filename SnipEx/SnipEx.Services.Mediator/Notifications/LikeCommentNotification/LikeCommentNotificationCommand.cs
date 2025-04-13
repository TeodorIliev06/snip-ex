namespace SnipEx.Services.Mediator.Notifications.LikeCommentNotification
{
    using MediatR;

    public record LikeCommentNotificationCommand(Guid RecipientId, Guid ActorId,
        Guid RelatedEntityId, string Message) : IRequest;
}
