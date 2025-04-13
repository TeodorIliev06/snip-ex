namespace SnipEx.Services.Mediator.Comments.CommentLiked
{
    using MediatR;

    public record CommentLikedEvent(Guid CommentGuid, Guid ActorGuid) : INotification;
}
