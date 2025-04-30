namespace SnipEx.Services.Mediator.Notifications.CreateConnectionNotification
{
    using MediatR;

    public record CreateConnectionNotificationCommand(Guid RecipientId, Guid ActorId, string Message) : IRequest;
}
