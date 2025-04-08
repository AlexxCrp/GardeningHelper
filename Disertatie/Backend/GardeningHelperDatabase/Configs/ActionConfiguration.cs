using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    public class ActionConfiguration : IEntityTypeConfiguration<Entities.PlantAction>
    {
        public void Configure(EntityTypeBuilder<Entities.PlantAction> builder)
        {
            builder.HasKey(a => a.Id); // Primary key

            // Configure action data
            builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Description).HasMaxLength(1000);
            builder.Property(a => a.TriggerStatus).IsRequired();
        }
    }
}
