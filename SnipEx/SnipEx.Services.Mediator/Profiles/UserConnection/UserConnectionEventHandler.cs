namespace SnipEx.Services.Mediator.Profiles.UserConnection
{
    using MediatR;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Notifications.CreateConnectionNotification;

    public class UserConnectionEventHandler(
        IRepository<ApplicationUser, Guid> userRepository,
        IRepository<UserConnection, object> userConnectionRepository,
        IMediator mediator) : INotificationHandler<UserConnectionEvent>
    {
        public async Task Handle(UserConnectionEvent notification, CancellationToken cancellationToken)
        {
            var actor = await userRepository.GetByIdAsync(notification.ActorGuid);
            if (actor == null)
            {
                return;
            }

            await mediator.Send(new CreateConnectionNotificationCommand(
                notification.TargetUserGuid,
                notification.ActorGuid,
                $"{actor.UserName} has connected with you"
            ), cancellationToken);
        }
    }
}
