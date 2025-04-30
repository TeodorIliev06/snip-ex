namespace SnipEx.Services.Mediator.Profiles.UserConnection
{
    using MediatR;

    public record UserConnectionEvent(Guid ActorGuid, Guid TargetUserGuid) : INotification;
}
