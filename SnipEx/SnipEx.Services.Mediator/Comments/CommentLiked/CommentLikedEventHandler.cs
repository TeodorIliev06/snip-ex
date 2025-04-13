namespace SnipEx.Services.Mediator.Comments.CommentLiked
{
    using MediatR;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Notifications.LikeCommentNotification;

    public class CommentLikedEventHandler(
        IRepository<Post, Guid> postRepository,
        IRepository<Comment, Guid> commentRepository,
        IRepository<ApplicationUser, Guid> userRepository,
        IMediator mediator) : INotificationHandler<CommentLikedEvent>
    {
        public async Task Handle(CommentLikedEvent notification, CancellationToken cancellationToken)
        {
            var comment = await commentRepository.GetByIdAsync(notification.CommentGuid);
            if (comment == null || !comment.UserId.HasValue ||
                comment.UserId == notification.ActorGuid)
            {
                return;
            }

            var post = await postRepository.GetByIdAsync(comment.PostId);
            var actor = await userRepository.GetByIdAsync(notification.ActorGuid);
            if (actor == null || post == null)
            {
                return;
            }

            await mediator.Send(new LikeCommentNotificationCommand(
                comment.UserId.Value,
                notification.ActorGuid,
                notification.CommentGuid,
                $"{actor.UserName} liked your comment on \"{comment.Post.Title}\""
            ), cancellationToken);
        }
    }
}
