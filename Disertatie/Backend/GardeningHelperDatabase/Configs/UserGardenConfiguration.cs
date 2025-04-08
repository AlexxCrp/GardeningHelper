using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    public class UserGardenConfiguration : IEntityTypeConfiguration<UserGarden>
    {
        public void Configure(EntityTypeBuilder<UserGarden> builder)
        {
            builder.HasKey(ug => ug.Id); // Primary key

            // Foreign key to User
            builder.HasOne(ug => ug.User)
                .WithMany(u => u.UserGardens)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation property to GardenPlant
            builder.HasMany(ug => ug.GardenPlants)
                .WithOne(gp => gp.UserGarden)
                .HasForeignKey(gp => gp.UserGardenId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
