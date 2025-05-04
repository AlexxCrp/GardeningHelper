using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    public class GardenPlantConfiguration : IEntityTypeConfiguration<GardenPlant>
    {
        public void Configure(EntityTypeBuilder<GardenPlant> builder)
        {
            builder.HasKey(gp => gp.Id); // Primary key

            // Foreign key to UserGarden
            builder.HasOne(gp => gp.UserGarden)
                .WithMany(ug => ug.GardenPlants)
                .HasForeignKey(gp => gp.UserGardenId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to Plant
            builder.HasOne(gp => gp.Plant)
                .WithMany(p => p.GardenPlants)
                .HasForeignKey(gp => gp.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure plant-specific data
            builder.Property(gp => gp.PositionX).IsRequired();
            builder.Property(gp => gp.PositionY).IsRequired();
            builder.Property(gp => gp.DaysToWateringCounter).IsRequired();
            builder.Property(gp => gp.LastWateredDate).IsRequired();
            builder.Property(gp => gp.LastRainfallDate).IsRequired();
            builder.Property(gp => gp.LastRainfallAmount).HasColumnType("decimal(5,2)");
            builder.Property(gp => gp.LastSoilMoisture).HasColumnType("decimal(5,2)");
            builder.Property(gp => gp.LastStatusCheckDate).IsRequired();

            // Configure status
            //builder.Property(p => p.Status).IsRequired();
        }
    }
}
