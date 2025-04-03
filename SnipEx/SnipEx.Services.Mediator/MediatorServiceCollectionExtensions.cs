namespace SnipEx.Services.Mediator
{
    using Microsoft.Extensions.DependencyInjection;

    using SnipEx.Services.Mediator.Comments.CommentAdded;

    public static class MediatorServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CommentAddedEventHandler).Assembly);
            });

            return services;
        }
    }
}
