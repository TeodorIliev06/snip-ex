namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;

    using static Common.EntityValidationConstants.Notification;

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder
                .HasKey(n => n.Id);

            builder
                .Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(MessageMaxLength);

            builder
                .Property(n => n.RelatedEntityType)
                .IsRequired()
                .HasMaxLength(EntityTypeMaxLength);

            // Enum configuration
            builder
                .Property(n => n.Type)
                .IsRequired()
                .HasConversion<string>();

            builder
                .Property(n => n.CreatedAt)
                .IsRequired();

            builder
                .HasOne(n => n.Recipient)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(n => n.Actor)
                .WithMany()
                .HasForeignKey(n => n.ActorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexing for performance
            builder
                .HasIndex(n => n.RecipientId);

            builder
                .HasIndex(n => new { n.RecipientId, n.IsRead });

            builder
                .HasIndex(n => n.CreatedAt);
        }
    }
}
