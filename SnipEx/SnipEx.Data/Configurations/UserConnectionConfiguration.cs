namespace SnipEx.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using SnipEx.Data.Models;
    using SnipEx.Data.Models.Enums;

    public class UserConnectionConfiguration : IEntityTypeConfiguration<UserConnection>
    {
        public void Configure(EntityTypeBuilder<UserConnection> builder)
        {
            builder
                .HasKey(uc => new { uc.UserId, uc.ConnectedUserId });

            builder
                .HasOne(uc => uc.User)
                .WithMany(u => u.Connections)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(uc => uc.ConnectedUser)
                .WithMany()
                .HasForeignKey(uc => uc.ConnectedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Property(uc => uc.Status)
                .HasDefaultValue(ConnectionStatus.Pending);
        }
    }
}
