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
            _logger.LogInformation("Gardening Helper Background Service is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("Gardening Helper Background Service is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Calculate the time until the next scheduled run (UTC)
                    var now = DateTime.UtcNow;
                    var scheduledTimeToday = now.Date.AddHours(_scheduleSettings.Hour).AddMinutes(_scheduleSettings.Minute);
                    var nextRunTime = scheduledTimeToday > now ? scheduledTimeToday : scheduledTimeToday.AddDays(1);

                    var delay = nextRunTime - now;

                    _logger.LogInformation($"Next scheduled run at: {nextRunTime.ToString("yyyy-MM-dd HH:mm:ss UTC")}");
                    _logger.LogInformation($"Delaying for: {delay.TotalHours:F1} hours ({delay.TotalMinutes:F1} minutes)");

                    // Wait until the scheduled time
                    await Task.Delay(delay, stoppingToken);

                    // Trigger the process for all users/plants
                    await TriggerDailyUpdatesAsync();

                    // The notification service call would typically happen here,
                    // after statuses are updated.
                    // Example: await _notificationService.SendNotificationsForUpdatedPlantsAsync();

                }
                catch (TaskCanceledException)
                {
                    // This happens when the application is shutting down gracefully
                    _logger.LogInformation("Gardening Helper Background Service task was cancelled.");
                    break; // Exit the loop
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while waiting for the next scheduled trigger.");
                    // Wait a bit before trying again to avoid tight loop on error
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }

            _logger.LogInformation("Gardening Helper Background Service has stopped.");
        }

        /// <summary>
        /// Performs the daily updates: fetch weather for all users, then update status for all plants.
        /// </summary>
        public async Task TriggerDailyUpdatesAsync()
        {
            _logger.LogInformation("Starting daily update process (Weather & Plant Status).");

            // Background services run as singletons, but scoped services (DbContext, UserManager, StatusService)
            // need a scope created for each unit of work.
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var weatherService = scope.ServiceProvider.GetRequiredService<WeatherService>();
                var plantStatusService = scope.ServiceProvider.GetRequiredService<PlantStatusService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<GardeningHelperDbContext>();

                try
                {
                    // --- Step 1: Fetch and Save Weather for All Users ---
                    _logger.LogInformation("Starting scheduled weather update for all users.");
                    var allUsers = await userManager.Users.ToListAsync();
                    _logger.LogInformation($"Found {allUsers.Count} users to update weather for.");

                    foreach (var user in allUsers)
                    {
                        try
                        {
                            // Assuming WeatherService handles logging internally
                            await weatherService.FetchAndSaveCurrentWeatherForUserAsync(user.UserName);
                        }
                        catch (Exception userWeatherEx)
                        {
                            _logger.LogError(userWeatherEx, $"An error occurred while updating weather for user ID: {user.Id}, Username: {user.UserName}. Skipping to next user.");
                        }
                    }
                    _logger.LogInformation("Scheduled weather update for all users finished.");


                    // --- Step 2: Update Status for All Garden Plants ---
                    _logger.LogInformation("Starting plant status update for all garden plants.");

                    // Fetch all GardenPlants. Include Plant details needed by the StatusService
                    var allGardenPlants = await dbContext.GardenPlants
                                                         .Include(gp => gp.Plant) // StatusService needs Plant thresholds
                                                         .Include(gp => gp.UserGarden) // StatusService needs UserGarden.UserId for weather
                                                         .ToListAsync();

                    _logger.LogInformation($"Found {allGardenPlants.Count} garden plants to assess.");

                    foreach (var gardenPlant in allGardenPlants)
                    {
                        try
                        {
                            // Update status for each plant
                            await plantStatusService.UpdatePlantStatusAsync(gardenPlant.Id);
                        }
                        catch (Exception plantStatusEx)
                        {
                            _logger.LogError(plantStatusEx, $"An error occurred while updating status for GardenPlant ID: {gardenPlant.Id} (Plant: {gardenPlant.Plant?.Name ?? "Unknown"}). Skipping to next plant.");
                        }
                    }

                    _logger.LogInformation("Plant status update for all garden plants finished.");

                    // --- Step 3: Trigger Notifications (Placeholder) ---
                    // This would be where you call a notification service
                    // Example: var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    // Example: await notificationService.GenerateAndSendNotificationsAsync();
                    _logger.LogInformation("Notification trigger point reached (Notification service call omitted in this example).");


                }
                catch (Exception scopeEx)
                {
                    _logger.LogError(scopeEx, "An error occurred within the service scope during the daily update process.");
                }
            }

            _logger.LogInformation("Daily update process completed.");
        }

        // The RecordWateringAsync method and others would still live in the PlantStatusService,
        // as they are triggered by specific user actions, not the daily background service.
    }
}