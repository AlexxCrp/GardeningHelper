using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    public class PlantConfiguration : IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.HasKey(p => p.Id); // Primary key
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.CareInstructions).HasMaxLength(1000);
            builder.Property(p => p.SunlightRequirements).HasMaxLength(200);
            builder.Property(p => p.SoilType).HasMaxLength(100);
            builder.Property(p => p.GrowthPeriod).HasMaxLength(100);
            builder.Property(p => p.HarvestTime).HasMaxLength(100);
            builder.Property(p => p.Image).HasMaxLength(500);

            // Configure thresholds
            builder.Property(p => p.MinTemperature).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MaxTemperature).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MinHumidity).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MaxHumidity).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MinRainfall).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MaxRainfall).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MinSoilMoisture).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MaxSoilMoisture).HasColumnType("decimal(5,2)");

            // Configure watering thresholds
            builder.Property(p => p.WateringThresholdDays).IsRequired();
            builder.Property(p => p.WateringThresholdRainfall).HasColumnType("decimal(5,2)");

            // Configure status
            builder.Property(p => p.Status).IsRequired();

            // Navigation property to UserInput
            builder.HasMany(p => p.UserInputs)
                .WithOne(ui => ui.Plant)
                .HasForeignKey(ui => ui.PlantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
