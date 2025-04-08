using GardeningHelperDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GardeningHelperDatabase.Configs
{
    public class UserInputConfiguration : IEntityTypeConfiguration<UserInput>
    {
        public void Configure(EntityTypeBuilder<UserInput> builder)
        {
            builder.HasKey(ui => ui.Id); // Primary key

            // Foreign key to User
            builder.HasOne(ui => ui.User)
                .WithMany(u => u.UserInputs)
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to Plant
            builder.HasOne(ui => ui.Plant)
                .WithMany(p => p.UserInputs)
                .HasForeignKey(ui => ui.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure user input data
            builder.Property(ui => ui.SoilMoisture).HasColumnType("decimal(5,2)");
            builder.Property(ui => ui.GrowthStage).HasMaxLength(100);
            builder.Property(ui => ui.Observations).HasMaxLength(1000);
            builder.Property(ui => ui.InputDate).IsRequired();
        }
    }
}
