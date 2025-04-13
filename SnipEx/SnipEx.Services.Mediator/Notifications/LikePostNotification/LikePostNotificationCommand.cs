namespace SnipEx.Services.Mediator.Notifications.LikePostNotification
{
    using MediatR;

    public record LikePostNotificationCommand(Guid RecipientId, Guid ActorId,
        Guid RelatedEntityId, string Message) : IRequest;
}
