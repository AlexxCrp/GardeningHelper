using GardeningHelperDatabase;
using GardeningHelperDatabase.Entities;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GardeningHelperAPI.Services.Weather
{
    public class WeatherService
    {
        private readonly GardeningHelperDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly WeatherAPIClient _weatherApiClient;
        private readonly ILogger<WeatherService> _logger; // Optional

        public WeatherService(GardeningHelperDbContext dbContext, UserManager<User> userManager, WeatherAPIClient weatherApiClient, ILogger<WeatherService> logger = null)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _weatherApiClient = weatherApiClient;
            _logger = logger; // Inject logger if configured
        }

        /// <summary>
        /// Fetches and saves current weather data for the user's garden location.
        /// </summary>
        /// <param name="userName">The ID of the user.</param>
        /// <returns>True if weather data was successfully fetched and saved, false otherwise.</returns>
        public async Task<bool> FetchAndSaveCurrentWeatherForUserAsync(string userName)
        {
            // 1. Find the user
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger?.LogWarning($"User with ID {userName} not found.");
                return false;
            }

            // 2. Find the user's garden to get location data
            // Assuming a user has at most one garden, or we take the first one found.
            // If a user can have multiple gardens and weather is per-garden,
            // you would need to modify this to get a specific garden by ID.
            var garden = await _dbContext.UserGardens
                .Include(x => x.User)
                .FirstOrDefaultAsync(g => g.User.UserName == userName);

            if (garden?.Latitude == null || garden?.Longitude == null)
            {
                _logger?.LogWarning($"Garden not found or location (Latitude/Longitude) is missing for user ID {userName}. Cannot fetch weather.");
                // Depending on requirements, you might create a default garden or inform the user.
                return false;
            }

            // 3. Fetch weather data from OpenWeatherMap
            var weatherDataFromApi = await _weatherApiClient.GetCurrentWeatherAsync(garden.Latitude.Value, garden.Longitude.Value);

            if (weatherDataFromApi == null)
            {
                _logger?.LogError($"Failed to fetch weather data from API for user ID {userName}.");
                return false; // API call failed or returned no data
            }

            // 4. Map API data to UserWeatherData entity
            var userWeatherData = new UserWeatherData
            {
                UserId = user.Id,
                User = user,
                Date = DateTime.UtcNow, // Record when the data was fetched
                General = weatherDataFromApi.Weather[0].Main,
                Temperature = weatherDataFromApi.Main.Temp,
                Humidity = weatherDataFromApi.Main.Humidity,
                // OpenWeatherMap provides rain for 1h or 3h. Use 1h if available, otherwise 0.
                Rainfall = weatherDataFromApi.Rain?.D1h ?? 0.0 // Rainfall in mm for the last hour
            };

            // 5. Save weather data to the database
            try
            {
                _dbContext.UserWeatherData.Add(userWeatherData);
                await _dbContext.SaveChangesAsync();
                _logger?.LogInformation($"Successfully saved weather data for user ID {userName}.");
                return true;
            }
            catch (Exception dbEx)
            {
                _logger?.LogError(dbEx, $"Failed to save weather data to database for user ID {userName}.");
                // Handle database saving errors
                return false;
            }
        }

        // You might add other methods here, e.g., GetLatestWeatherDataForUserAsync(string userId)
        public async Task<UserWeatherData> GetLatestWeatherDataForUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger?.LogWarning($"User with ID {userId} not found when retrieving weather.");
                return null;
            }

            // Get the most recent weather entry for the user
            return await _dbContext.UserWeatherData
                                    .Where(wd => wd.UserId == userId)
                                    .OrderByDescending(wd => wd.Date)
                                    .FirstOrDefaultAsync();
        }
    }
}
