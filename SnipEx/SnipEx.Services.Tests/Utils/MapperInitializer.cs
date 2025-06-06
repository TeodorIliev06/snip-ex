namespace SnipEx.Services.Tests.Utils
{
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Tag;
    using SnipEx.Services.Data.Models;

    public static class MapperInitializer
    {
        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (!isInitialized)
            {
                var assemblies = new[]
                {
                    typeof(TagViewModel).Assembly,
                    typeof(Tag).Assembly,
                    typeof(TagService).Assembly
                };

                AutoMapperConfig.RegisterMappings(assemblies);
            }
        }
    }
}
