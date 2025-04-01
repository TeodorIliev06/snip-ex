namespace SnipEx.Services.Mediator.Comments.CommentAdded
{
    using MediatR;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Notifications.CreateCommentNotification;

    public class CommentAddedEventHandler(
        IRepository<Post, Guid> postRepository,
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

            await mediator.Send(new CreateCommentNotificationCommand(
                 post.UserId.Value,
                 notification.ActorGuid,
                 notification.CommentGuid,
                 "Someone commented on your post!"
             ), cancellationToken);
        }
    }
}
