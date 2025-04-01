namespace SnipEx.Services.Mediator.Notifications.CreateCommentNotification
{
    using MediatR;

    public record CreateCommentNotificationCommand(Guid RecipientId, Guid ActorId,
        Guid RelatedEntityId, string Message) : IRequest;
}
