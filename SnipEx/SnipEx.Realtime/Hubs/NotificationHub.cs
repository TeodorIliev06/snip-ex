namespace SnipEx.Realtime.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.Authorization;

    using static SnipEx.Common.SignalRConstants;

    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(Guid recipientId, string message)
        {
            await Clients.User(recipientId.ToString()).SendAsync(MethodNames.ReceiveNotification, message);
        }
    }
}
