namespace SnipEx.Services.Mediator.Comments.CommentAdded
{
    using MediatR;

    public record CommentAddedEvent(Guid CommentGuid, Guid PostGuid, Guid ActorGuid) : INotification;
}
