using GardeningHelperDatabase.Entities;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GardeningHelperDatabase.Seed
{
    public class SeedData
    {
        public static async Task SeedDatabase(GardeningHelperDbContext context, RoleManager<Role> roleManager)
        {
            await SeedRoles(roleManager);
            SeedPlants(context);
            context.SaveChanges();
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

            // Get the path to the PlantImages folder
            string plantImagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\GardeningHelperDatabase\\Seed\\PlantImages");

            // Iterate over the plants and load their respective images
            foreach (var plant in plants)
            {
                // Construct the path to the image file for the current plant
                string imageFilePath = Path.Combine(plantImagesFolderPath, $"{plant.Name}.png");

                if (File.Exists(imageFilePath))
                {
                    // Read the image file and convert it to a byte array
                    byte[] imageBytes = File.ReadAllBytes(imageFilePath);
                    plant.Image = imageBytes;
                }
                else
                {
                    // You can decide to set a default image or handle this case differently
                    plant.Image = null; // Or load a default image if needed
                }
            }

            context.Plants.AddRange(plants);
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
