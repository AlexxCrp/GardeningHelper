using GardeningHelperDatabase.Configs;
using GardeningHelperDatabase.Entities;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GardeningHelperDatabase
{
    public class GardeningHelperDbContext : IdentityDbContext
        <User, Role, string, IdentityUserClaim<string>,
        UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DbSet<Plant> Plants { get; set; }
        public DbSet<PlantDetails> PlantDetails { get; set; }
        public DbSet<UserGarden> UserGardens { get; set; }
        public DbSet<GardenPlant> GardenPlants { get; set; }
        public DbSet<UserWeatherData> UserWeatherData { get; set; }
        public DbSet<UserInput> UserInputs { get; set; }
        public DbSet<PlantAction> Actions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public GardeningHelperDbContext(DbContextOptions<GardeningHelperDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new PlantConfiguration());
            modelBuilder.ApplyConfiguration(new PlantDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new UserGardenConfiguration());
            modelBuilder.ApplyConfiguration(new GardenPlantConfiguration());
            modelBuilder.ApplyConfiguration(new UserWeatherDataConfiguration());
            modelBuilder.ApplyConfiguration(new UserInputConfiguration());
            modelBuilder.ApplyConfiguration(new ActionConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        }
    }
}
