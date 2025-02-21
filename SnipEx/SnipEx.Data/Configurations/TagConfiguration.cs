namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;
    using static Common.EntityValidationConstants.Tag;

    internal class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder
                .HasKey(t => t.Id);

            builder
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder
                .Property(t => t.Description)
                .IsRequired(false)
                .HasMaxLength(DescriptionMaxLength);
        }
    }
}
