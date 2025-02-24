namespace SnipEx.Web.Infrastructure
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories;
    using SnipEx.Data.Repositories.Contracts;

    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services, Assembly modelsAssembly)
        {
            Type[] typesToExclude = { typeof(ApplicationUser) };
            Type[] modelsTypes = modelsAssembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract && !t.IsInterface &&
                    !typeof(Attribute).IsAssignableFrom(t))
                .ToArray();

            foreach (Type type in modelsTypes)
            {
                if (!typesToExclude.Contains(type))
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

    }
}