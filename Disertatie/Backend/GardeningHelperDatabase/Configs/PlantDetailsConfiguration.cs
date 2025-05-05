using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    internal class PlantDetailsConfiguration : IEntityTypeConfiguration<PlantDetails>
    {
        public void Configure(EntityTypeBuilder<PlantDetails> builder)
        {
            builder.HasOne(pd => pd.Plant)
                .WithOne(p => p.Details)
                .HasForeignKey<PlantDetails>(pd => pd.PlantId);
        }
    }
}
