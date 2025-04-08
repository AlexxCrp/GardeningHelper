using GardeningHelperDatabase.Entities;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GardeningHelperDatabase.Seed
{
    public class SeedData
    {
        public static async Task
SeedDatabase(GardeningHelperDbContext context, RoleManager<Role> roleManager)
        {
            await SeedRoles(roleManager);
            SeedPlants(context);
        }

        public static void SeedPlants(GardeningHelperDbContext context)
        {
            if (context.Plants.Any())
            {
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var plants = JsonSerializer.Deserialize<List<Plant>>(Constants.PlantsJson, options);

            context.Plants.AddRange(plants);
            context.SaveChanges();
        }

        public static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            // Check if the roles already exist
            if (await roleManager.RoleExistsAsync(Constants.Admin))
            {
                return; // Roles have already been seeded
            }

            // Create and add the Admin role
            var adminRole = new Role { Name = Constants.Admin };
            await roleManager.CreateAsync(adminRole);

            // Create and add the User role
            var userRole = new Role { Name = Constants.User };
            await roleManager.CreateAsync(userRole);
        }
    }
}
