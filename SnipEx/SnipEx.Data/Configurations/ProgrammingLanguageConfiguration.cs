namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;
    using SnipEx.Data.Extensions;

    using static Common.EntityValidationConstants.ProgrammingLanguage;

    public class ProgrammingLanguageConfiguration : IEntityTypeConfiguration<ProgrammingLanguage>
    {
        public void Configure(EntityTypeBuilder<ProgrammingLanguage> builder)
        {
            builder
                .HasKey(pl => pl.Id);

            builder
                .Property(pl => pl.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder
                .Property(pl => pl.FileExtension)
                .IsRequired()
                .HasMaxLength(FileExtensionMaxLength);

            builder
                .SeedDataFromJson("programmingLanguages.json");
        }
    }
}
