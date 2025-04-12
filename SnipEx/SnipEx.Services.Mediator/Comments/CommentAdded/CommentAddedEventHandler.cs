namespace SnipEx.Services.Mediator.Comments.CommentAdded
{
    using MediatR;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Notifications.CreateCommentNotification;

    public class CommentAddedEventHandler(
        IRepository<Post, Guid> postRepository,
        IRepository<ApplicationUser, Guid> userRepository,
        IMediator mediator) : INotificationHandler<CommentAddedEvent>
    {
        public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(notification.PostGuid);
            if (post == null || !post.UserId.HasValue ||
                post.UserId == notification.ActorGuid)
            {
                return;
            }

            var actor = await userRepository.GetByIdAsync(notification.ActorGuid);
            if (actor == null)
            {
                return;
            }

            await mediator.Send(new CreateCommentNotificationCommand(
                 post.UserId.Value,
                 notification.ActorGuid,
                 notification.CommentGuid,
                 $"{actor.UserName} commented on your snippet \"{post.Title}\""
             ), cancellationToken);
        }
    }
}
