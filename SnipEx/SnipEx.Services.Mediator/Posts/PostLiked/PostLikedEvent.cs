namespace SnipEx.Services.Mediator.Posts.PostLiked
{
    using MediatR;

    public record PostLikedEvent(Guid PostGuid, Guid ActorGuid) : INotification;
}
