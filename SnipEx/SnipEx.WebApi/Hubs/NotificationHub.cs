namespace SnipEx.WebApi.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    using static Common.SignalRConstants;

    public class NotificationHub : Hub
    {
        public async Task SendNotification(Guid recipientId, string message)
        {
            await Clients.User(recipientId.ToString()).SendAsync(MethodNames.ReceiveNotification, message);
        }
    }
}
