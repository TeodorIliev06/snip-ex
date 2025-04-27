namespace SnipEx.Services.Mediator.Comments.ReplyAdded
{
    using MediatR;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Notifications.CreateReplyNotification;

    public class ReplyAddedEventHandler(
        IRepository<Comment, Guid> commentRepository,
        IRepository<ApplicationUser, Guid> userRepository,
        IMediator mediator) : INotificationHandler<ReplyAddedEvent>
    {
        public async Task Handle(ReplyAddedEvent notification, CancellationToken cancellationToken)
        {
            var comment = await commentRepository.GetByIdAsync(notification.ParentCommentGuid);
            if (comment == null || !comment.UserId.HasValue ||
                comment.UserId == notification.ActorGuid)
            {
                return;
            }

            var actor = await userRepository.GetByIdAsync(notification.ActorGuid);
            if (actor == null)
            {
                return;
            }

            var replyMessage = $"{actor.UserName} replied to your comment";
            var mentionMessage = $"{actor.UserName} mentioned you in a comment";

            var outputMessage = notification.IsMention
                ? mentionMessage
                : replyMessage;

            await mediator.Send(new CreateReplyNotificationCommand(
                comment.UserId.Value,
                notification.ActorGuid,
                notification.ParentCommentGuid,
                outputMessage,
                notification.IsMention
            ), cancellationToken);
        }
    }
}
