namespace SnipEx.Web.Infrastructure
{
    using System.Reflection;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi.Models;
    using Microsoft.Extensions.DependencyInjection;

    using SnipEx.Data.Repositories;
    using SnipEx.Web.Infrastructure.Filters;
    using SnipEx.Data.Repositories.Contracts;

    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services, Assembly modelsAssembly)
        {
            Type[] typesToExclude = {  };
            Type[] modelsTypes = modelsAssembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract && !t.IsInterface &&
                    !typeof(Attribute).IsAssignableFrom(t))
                .ToArray();

            foreach (Type type in modelsTypes)
            {
                if (!typesToExclude.Contains(type) && !type.IsEnum)
                {
                    Type repositoryInterfaceType = typeof(IRepository<,>);
                    Type repositoryInstanceType = typeof(BaseRepository<,>);

                    PropertyInfo? idPropInfo = type
                        .GetProperties()
                        .SingleOrDefault(p => p.Name.ToLower() == "id");

                    Type[] constructArgs = new Type[2];
                    constructArgs[0] = type;

                    if (idPropInfo == null)
                    {
                        constructArgs[1] = typeof(object);
                    }
                    else
                    {
                        constructArgs[1] = idPropInfo.PropertyType;
                    }

                    repositoryInterfaceType = repositoryInterfaceType.MakeGenericType(constructArgs);
                    repositoryInstanceType = repositoryInstanceType.MakeGenericType(constructArgs);

                    services.AddScoped(repositoryInterfaceType, repositoryInstanceType);
                }
            }
        }

        public static void RegisterUserDefinedServices(this IServiceCollection services, Assembly servicesAssembly)
        {
            Type[] serviceInterfaceTypes = servicesAssembly
                .GetTypes()
                .Where(t => t.IsInterface)
                .ToArray();
            Type[] serviceTypes = servicesAssembly
                .GetTypes()
                .Where(t =>
                    !t.IsInterface && !t.IsAbstract &&
                    t.Name.ToLower().EndsWith("service"))
                .ToArray();

            foreach (var serviceInterfaceType in serviceInterfaceTypes)
            {
                Type? serviceType = serviceTypes
                    .SingleOrDefault(t =>
                        String.Equals("I" + t.Name, serviceInterfaceType.Name, StringComparison.OrdinalIgnoreCase));

                if (serviceType == null)
                {
                    throw new NullReferenceException($"Service type could not be obtained {serviceInterfaceType.Name}");
                }

                services.AddScoped(serviceInterfaceType, serviceType);
            }
        }

        public static IServiceCollection AddSwaggerApi(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SnipEx.WebApi",
                    Version = "v1",
                    Description = "API for SnipEx app"
                });

                c.DocInclusionPredicate((docName, description) =>
                {
                    // Only include controllers with [ApiController] attribute
                    var controllerActionDescriptor = description.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    return controllerActionDescriptor?.ControllerTypeInfo.GetCustomAttributes(typeof(ApiControllerAttribute), true).Any() == true;
                });

                c.OperationFilter<FileUploadOperationFilter>();
            });

            return services;
        }
    }
}