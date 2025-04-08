using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id); // Primary key

            // Foreign key to User
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure notification data
            builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            builder.Property(n => n.NotificationDate).IsRequired();
            builder.Property(n => n.IsRead).IsRequired();
        }
    }
}
