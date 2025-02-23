namespace SnipEx.Data.Extensions
{
    using System.Text.Json;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class EntityTypeBuilderExtensions
    {
        public static void SeedDataFromJson<T>(this EntityTypeBuilder<T> builder, string relativePath)
            where T : class
        {
            string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SnipEx.Data",
                "Datasets", relativePath);
            string data = File.ReadAllText(path);

            var entities = JsonSerializer.Deserialize<List<T>>(data);
            if (entities != null)
            {
                builder.HasData(entities);
            }
        }
    }
}