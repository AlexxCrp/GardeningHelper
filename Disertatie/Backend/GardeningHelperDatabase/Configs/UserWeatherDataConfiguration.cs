using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    public class UserWeatherDataConfiguration : IEntityTypeConfiguration<UserWeatherData>
    {
        public void Configure(EntityTypeBuilder<UserWeatherData> builder)
        {
            builder.HasKey(uw => uw.Id); // Primary key

            // Foreign key to User
            builder.HasOne(uw => uw.User)
                .WithMany(u => u.UserWeatherData)
                .HasForeignKey(uw => uw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure weather data
            builder.Property(uw => uw.Date).IsRequired();
            builder.Property(uw => uw.Temperature).HasColumnType("decimal(5,2)");
            builder.Property(uw => uw.Humidity).HasColumnType("decimal(5,2)");
            builder.Property(uw => uw.Rainfall).HasColumnType("decimal(5,2)");
        }
    }
}
