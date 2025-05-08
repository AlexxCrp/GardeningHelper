using GardeningHelperAPI.Services.Weather;
using GardeningHelperDatabase;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GardeningHelperAPI.Services
{
    public class GardenBackgroundService : BackgroundService
    {
        private readonly ILogger<GardenBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory; // Used to create a new scope for each run
        private readonly WeatherUpdateScheduleSettings _scheduleSettings;

        public GardenBackgroundService(
            ILogger<GardenBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<WeatherUpdateScheduleSettings> scheduleSettings)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _scheduleSettings = scheduleSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Weather Update Background Service is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("Weather Update Background Service is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Calculate the time until the next scheduled run (UTC)
                    var now = DateTime.UtcNow;
                    var scheduledTimeToday = now.Date.AddHours(_scheduleSettings.Hour).AddMinutes(_scheduleSettings.Minute);
                    var nextRunTime = scheduledTimeToday > now ? scheduledTimeToday : scheduledTimeToday.AddDays(1);

                    var delay = nextRunTime - now;

                    _logger.LogInformation($"Next scheduled weather update at: {nextRunTime.ToString("yyyy-MM-dd HH:mm:ss UTC")}");
                    _logger.LogInformation($"Delaying for: {delay.TotalHours:F1} hours ({delay.TotalMinutes:F1} minutes)");

                    // Wait until the scheduled time
                    await Task.Delay(delay, stoppingToken);

                    // Trigger the update process
                    await TriggerWeatherUpdateForAllUsersAsync();
                    //Add here Status Method call
                    //Add here notification service call

                }
                catch (TaskCanceledException)
                {
                    // This happens when the application is shutting down gracefully
                    _logger.LogInformation("Weather Update Background Service task was cancelled.");
                    break; // Exit the loop
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while waiting for the next weather update trigger.");
                    // Wait a bit before trying again to avoid tight loop on error
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }

            _logger.LogInformation("Weather Update Background Service has stopped.");
        }

        /// <summary>
        /// Performs the weather update for all users. Can be called manually or by the scheduler.
        /// </summary>
        public async Task TriggerWeatherUpdateForAllUsersAsync()
        {
            _logger.LogInformation("Starting scheduled weather update for all users.");

            // Background services run as singletons, but UserManager and DbContext are scoped.
            // We need to create a service scope manually to get scoped services.
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var weatherService = scope.ServiceProvider.GetRequiredService<WeatherService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<GardeningHelperDbContext>();

                try
                {
                    var allUsers = await userManager.Users.ToListAsync();


                    _logger.LogInformation($"Found {allUsers.Count} users to update.");

                    foreach (var user in allUsers)
                    {
                        try
                        {
                            _logger.LogInformation($"Updating weather for user ID: {user.Id}, Username: {user.UserName}");
                            bool success = await weatherService.FetchAndSaveCurrentWeatherForUserAsync(user.UserName);

                            if (success)
                            {
                                _logger.LogInformation($"Successfully updated weather for user ID: {user.Id}");
                            }
                            else
                            {
                                _logger.LogWarning($"Failed to update weather for user ID: {user.Id}. See previous logs for details.");
                            }
                        }
                        catch (Exception userEx)
                        {
                            _logger.LogError(userEx, $"An error occurred while updating weather for user ID: {user.Id}");
                        }
                    }

                    _logger.LogInformation("Scheduled weather update for all users finished.");

                }
                catch (Exception scopeEx)
                {
                    _logger.LogError(scopeEx, "An error occurred within the service scope during the weather update.");
                }
            }
        }

        //Add here Status Method call
        //Add here notification service call
    }
}
