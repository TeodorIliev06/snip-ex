namespace SnipEx.Services.Mediator.Posts.PostLiked
{
    using MediatR;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Notifications.LikePostNotification;

    public class PostLikedEventHandler(
        IRepository<Post, Guid> postRepository,
        IRepository<ApplicationUser, Guid> userRepository,
        IMediator mediator) : INotificationHandler<PostLikedEvent>
    {
        public async Task Handle(PostLikedEvent notification, CancellationToken cancellationToken)
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

            await mediator.Send(new LikePostNotificationCommand(
                post.UserId.Value,
                notification.ActorGuid,
                notification.PostGuid,
                $"{actor.UserName} liked your snippet \"{post.Title}\""
            ), cancellationToken);
        }
    }
}
