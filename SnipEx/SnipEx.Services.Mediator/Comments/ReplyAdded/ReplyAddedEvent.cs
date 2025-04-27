namespace SnipEx.Services.Mediator.Comments.ReplyAdded
{
    using MediatR;

    public record ReplyAddedEvent(Guid ReplyGuid, Guid ParentCommentGuid, Guid ActorGuid, bool IsMention) : INotification;

}
