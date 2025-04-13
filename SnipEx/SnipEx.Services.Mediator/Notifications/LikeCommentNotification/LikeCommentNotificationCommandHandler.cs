namespace SnipEx.Services.Mediator.Notifications.LikeCommentNotification
{
    using MediatR;
    using Microsoft.AspNetCore.SignalR;

    using SnipEx.Data.Models;
    using SnipEx.Realtime.Hubs;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Data.Repositories.Contracts;

    using static Common.SignalRConstants;

    public class LikeCommentNotificationCommandHandler(
        IRepository<Notification, Guid> notificationRepository,
        IHubContext<NotificationHub> hubContext) : IRequestHandler<LikeCommentNotificationCommand>
    {
        public async Task Handle(LikeCommentNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Message = request.Message,
                Type = NotificationType.CommentLike,
                RecipientId = request.RecipientId,
                ActorId = request.ActorId,
                RelatedEntityId = request.RelatedEntityId,
                RelatedEntityType = nameof(Comment),
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await notificationRepository.AddAsync(notification);
            await notificationRepository.SaveChangesAsync();

            // Send real-time notification
            await hubContext.Clients.User(request.RecipientId.ToString())
                .SendAsync(MethodNames.ReceiveNotification, notification, cancellationToken: cancellationToken);
        }
    }
}
