namespace SnipEx.Realtime
{
    using Microsoft.Extensions.DependencyInjection;

    using SnipEx.Realtime.Hubs;

    public static class RealtimeServiceCollectionExtensions
    {
        public static IServiceCollection AddRealtimeServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<NotificationHub>();

            return services;
        }
    }
}
