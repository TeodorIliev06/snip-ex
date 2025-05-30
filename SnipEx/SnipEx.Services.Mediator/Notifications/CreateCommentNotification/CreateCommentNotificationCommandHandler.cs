namespace SnipEx.Services.Mediator.Notifications.CreateCommentNotification
{
    using MediatR;
    using Microsoft.AspNetCore.SignalR;

    using SnipEx.Data.Models;
    using SnipEx.Realtime.Hubs;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Web.ViewModels.DTOs;
    using SnipEx.Data.Repositories.Contracts;

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

            var notificationDto = new NotificationDto
            {
                Message = notification.Message,
                RecipientId = notification.RecipientId.ToString(),
                ActorId = notification.ActorId.ToString()
            };

            await notificationRepository.AddAsync(notification);
            await notificationRepository.SaveChangesAsync();

            // Send real-time notification
            await hubContext.Clients.User(request.RecipientId.ToString())
                .SendAsync(MethodNames.ReceiveNotification, notificationDto, cancellationToken: cancellationToken);
        }
    }
}
