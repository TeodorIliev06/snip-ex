namespace SnipEx.Services.Mediator.Notifications.CreateCommentNotification
{
    using MediatR;
    using Microsoft.AspNetCore.SignalR;

    using SnipEx.WebApi.Hubs;
    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Data.Models.Enums;

    using static Common.SignalRConstants;

    public class CreateCommentNotificationCommandHandler(
        IRepository<Notification, Guid> notificationRepository,
        IHubContext<NotificationHub> hubContext) : IRequestHandler<CreateCommentNotificationCommand>
    {
        public async Task Handle(CreateCommentNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Message = request.Message,
                Type = NotificationType.PostComment,
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
