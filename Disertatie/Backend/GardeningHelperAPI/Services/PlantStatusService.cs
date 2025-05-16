// GardeningHelperAPI/Services/PlantStatusService.cs
using DataExchange.Enums;
using GardeningHelperAPI.Services.Weather;
using GardeningHelperDatabase;
using Microsoft.EntityFrameworkCore;

namespace GardeningHelperAPI.Services
{
    public class PlantStatusService
    {
        private readonly GardeningHelperDbContext _dbContext;
        private readonly WeatherService _weatherService; // To get latest weather data
        private readonly ILogger<PlantStatusService> _logger;

        public PlantStatusService(
            GardeningHelperDbContext dbContext,
            WeatherService weatherService,
            ILogger<PlantStatusService> logger)
        {
            _dbContext = dbContext;
            _weatherService = weatherService;
            _logger = logger;
        }

        /// <summary>
        /// Assesses and updates the status of a specific GardenPlant based on its needs and current conditions.
        /// </summary>
        /// <param name="gardenPlantId">The ID of the GardenPlant to assess.</param>
        /// <returns>The updated StatusEnum for the plant, or null if processing failed.</returns>
        public async Task<StatusEnum?> UpdatePlantStatusAsync(int gardenPlantId)
        {
            _logger.LogInformation($"Assessing status for GardenPlant ID: {gardenPlantId}");

            // 1. Fetch GardenPlant and related data
            var gardenPlant = await _dbContext.GardenPlants
                .Include(gp => gp.Plant) // Include the base Plant info for thresholds
                .Include(gp => gp.UserGarden) // Include UserGarden to get UserId for weather
                .FirstOrDefaultAsync(gp => gp.Id == gardenPlantId);

            if (gardenPlant == null)
            {
                _logger.LogWarning($"GardenPlant with ID {gardenPlantId} not found.");
                return null;
            }

            // Store the current status as the previous status BEFORE calculating the new one
            gardenPlant.PreviousStatus = gardenPlant.Status;
            gardenPlant.StatusChangeReason = null; // Clear previous reason

            var plant = gardenPlant.Plant;
            if (plant == null)
            {
                _logger.LogError($"Plant details not found for GardenPlant ID: {gardenPlantId}. Cannot assess status.");
                return null;
            }

            // 2. Get the latest weather data for the user's garden location
            var latestWeatherData = await _weatherService.GetLatestWeatherDataForUserAsync(gardenPlant.UserGarden.UserId);

            if (latestWeatherData == null)
            {
                _logger.LogWarning($"No weather data found for user ID {gardenPlant.UserGarden.UserId}. Status assessment may be incomplete for GardenPlant ID: {gardenPlantId}.");
                // Proceed with assessment using only available data (watering counter, soil moisture)
            }
            else
            {
                _logger.LogInformation($"Latest weather data retrieved (Date: {latestWeatherData.Date}, Temp: {latestWeatherData.Temperature}, Humidity: {latestWeatherData.Humidity}, Rainfall: {latestWeatherData.Rainfall}) for GardenPlant ID: {gardenPlantId}.");
            }


            // 3. Determine the plant's status based on thresholds and conditions
            StatusEnum newStatus = StatusEnum.Normal;

            // --- Prioritize AtRisk status for critical conditions ---

            bool isAtRisk = false;
            string atRiskReason = ""; // Reason for being AtRisk

            if (latestWeatherData != null)
            {
                // Temperature check
                if (latestWeatherData.Temperature < plant.MinTemperature)
                {
                    isAtRisk = true;
                    atRiskReason = $"Temperature ({latestWeatherData.Temperature}°C) below minimum threshold ({plant.MinTemperature}°C).";
                    _logger.LogWarning($"Plant {plant.Name} (ID: {gardenPlant.Id}) AtRisk: {atRiskReason}");
                }
                else if (latestWeatherData.Temperature > plant.MaxTemperature)
                {
                    isAtRisk = true;
                    atRiskReason = $"Temperature ({latestWeatherData.Temperature}°C) above maximum threshold ({plant.MaxTemperature}°C).";
                    _logger.LogWarning($"Plant {plant.Name} (ID: {gardenPlant.Id}) AtRisk: {atRiskReason}");
                }

                // Humidity check (if not already AtRisk from temperature)
                if (!isAtRisk && (latestWeatherData.Humidity < plant.MinHumidity || latestWeatherData.Humidity > plant.MaxHumidity))
                {
                    isAtRisk = true;
                    atRiskReason = $"Humidity ({latestWeatherData.Humidity}%) outside acceptable range ({plant.MinHumidity}-{plant.MaxHumidity}%).";
                    _logger.LogWarning($"Plant {plant.Name} (ID: {gardenPlant.Id}) AtRisk: {atRiskReason}");
                }

                // Excessive Rainfall check (if not already AtRisk)
                if (!isAtRisk && latestWeatherData.Rainfall > plant.MaxRainfall)
                {
                    isAtRisk = true;
                    atRiskReason = $"Excessive rainfall detected today ({latestWeatherData.Rainfall}mm), maximum recommended is ({plant.MaxRainfall}mm).";
                    _logger.LogWarning($"Plant {plant.Name} (ID: {gardenPlant.Id}) AtRisk: {atRiskReason}");
                }
            }

            // Soil Moisture check (if data is available and not already AtRisk)
            // Assuming LastSoilMoisture > 0 means valid sensor data
            if (!isAtRisk && gardenPlant.LastSoilMoisture > 0 && gardenPlant.LastSoilMoisture > plant.MaxSoilMoisture)
            {
                isAtRisk = true;
                atRiskReason = $"Soil moisture ({gardenPlant.LastSoilMoisture}%) above maximum threshold ({plant.MaxSoilMoisture}%) (potential overwatering).";
                _logger.LogWarning($"Plant {plant.Name} (ID: {gardenPlant.Id}) AtRisk: {atRiskReason}");
            }


            if (isAtRisk)
            {
                newStatus = StatusEnum.AtRisk;
                gardenPlant.StatusChangeReason = atRiskReason; // Set the reason
            }
            else
            {
                // --- If not AtRisk, check for NeedsWatering ---

                bool needsWatering = false;
                string needsWateringReason = ""; // Reason for needing watering

                // Check Soil Moisture first (most direct indicator)
                // Assuming LastSoilMoisture > 0 means valid sensor data
                if (gardenPlant.LastSoilMoisture > 0 && gardenPlant.LastSoilMoisture < plant.MinSoilMoisture)
                {
                    needsWatering = true;
                    needsWateringReason = $"Soil moisture ({gardenPlant.LastSoilMoisture}%) is below the minimum required ({plant.MinSoilMoisture}%).";
                    _logger.LogInformation($"Plant {plant.Name} (ID: {gardenPlant.Id}) NeedsWatering: {needsWateringReason}");
                }
                else
                {
                    // If soil moisture isn't the issue, check time-based counter, adjusted by rain
                    // Check if significant rain occurred *today* to reset the counter
                    bool significantRainToday = false;
                    if (latestWeatherData != null && latestWeatherData.Rainfall >= plant.WateringThresholdRainfall)
                    {
                        // Check if this weather data is recent enough to count for today
                        // Assuming daily updates, comparing dates should be sufficient
                        if (latestWeatherData.Date.Date == DateTime.UtcNow.Date) // Using UtcNow for comparison consistency
                        {
                            significantRainToday = true;
                            _logger.LogInformation($"Plant {plant.Name} (ID: {gardenPlant.Id}): Significant rain today ({latestWeatherData.Rainfall}mm >= {plant.WateringThresholdRainfall}mm). Resetting watering counter.");

                            // Reset watering counter as rain counts as watering
                            gardenPlant.DaysToWateringCounter = 0;
                            gardenPlant.LastRainfallDate = latestWeatherData.Date; // Record date of significant rain
                            gardenPlant.LastRainfallAmount = latestWeatherData.Rainfall;
                            // Do NOT update LastWateredDate here - that's for manual watering
                        }
                        else
                        {
                            _logger.LogWarning($"Plant {plant.Name} (ID: {gardenPlant.Id}): Latest weather data is not from today ({latestWeatherData.Date.Date} vs {DateTime.UtcNow.Date}). Cannot determine if significant rain occurred today.");
                        }
                    }

                    // If no significant rain today, increment the counter for days since last "watering" event
                    // Only increment if the last status check wasn't today
                    if (!significantRainToday)
                    {
                        if (gardenPlant.LastStatusCheckDate.Date != DateTime.UtcNow.Date) // Prevent double increment on same day runs
                        {
                            gardenPlant.DaysToWateringCounter++;
                            _logger.LogInformation($"Plant {plant.Name} (ID: {gardenPlant.Id}): No significant rain today. Incrementing watering counter to {gardenPlant.DaysToWateringCounter}.");
                        }
                        else
                        {
                            _logger.LogInformation($"Plant {plant.Name} (ID: {gardenPlant.Id}): Status already checked today ({gardenPlant.LastStatusCheckDate.Date}). Watering counter not incremented based on time.");
                        }
                    }


                    // Check if the counter has reached the watering threshold
                    if (gardenPlant.DaysToWateringCounter >= plant.WateringThresholdDays)
                    {
                        needsWatering = true;
                        needsWateringReason = $"Has gone {gardenPlant.DaysToWateringCounter} days without watering, exceeding the recommended threshold of {plant.WateringThresholdDays} days.";
                        _logger.LogInformation($"Plant {plant.Name} (ID: {gardenPlant.Id}) NeedsWatering: {needsWateringReason}");
                    }
                }

                if (needsWatering)
                {
                    newStatus = StatusEnum.NeedsWatering;
                    gardenPlant.StatusChangeReason = needsWateringReason; // Set the reason
                }
                else
                {
                    // If not AtRisk and not NeedsWatering, it's Normal
                    newStatus = StatusEnum.Normal;
                    _logger.LogInformation($"Plant {plant.Name} (ID: {gardenPlant.Id}): Status set to Normal.");
                    // StatusChangeReason remains null
                }
            }

            // 4. Update GardenPlant entity
            gardenPlant.Status = newStatus;
            gardenPlant.LastStatusCheckDate = DateTime.UtcNow; // Record when status was last checked

            // 5. Save changes to the database
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated status for GardenPlant ID: {gardenPlantId} from {gardenPlant.PreviousStatus} to {newStatus}. Reason: {gardenPlant.StatusChangeReason ?? "N/A"}");
                return newStatus;
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, $"Failed to save status update for GardenPlant ID: {gardenPlantId}.");
                throw; // Re-throw or handle appropriately
            }
        }

        /// <summary>
        /// Call this method when a user manually waters a plant.
        /// </summary>
        /// <param name="gardenPlantId">The ID of the GardenPlant that was watered.</param>
        /// <param name="soilMoistureReading">Optional: Soil moisture reading after watering.</param>
        /// <returns>True if update was successful.</returns>
        public async Task<bool> RecordWateringAsync(int gardenPlantId, double? soilMoistureReading = null)
        {
            _logger.LogInformation($"Recording watering for GardenPlant ID: {gardenPlantId}");
            var gardenPlant = await _dbContext.GardenPlants
                 .Include(gp => gp.Plant) // Needed if you want to potentially trigger status update here too
                 .Include(gp => gp.UserGarden) // Needed if you want to potentially trigger status update here too
                 .FirstOrDefaultAsync(gp => gp.Id == gardenPlantId);


            if (gardenPlant == null)
            {
                _logger.LogWarning($"GardenPlant with ID {gardenPlantId} not found for watering record.");
                return false;
            }

            // Store current status before updating for watering
            gardenPlant.PreviousStatus = gardenPlant.Status;
            gardenPlant.LastWateredDate = DateTime.UtcNow;
            gardenPlant.DaysToWateringCounter = 0; // Reset counter after watering
            gardenPlant.StatusChangeReason = null; // Clear reason on watering

            if (soilMoistureReading.HasValue)
            {
                // Assuming soilMoistureReading > 0 means a valid reading
                gardenPlant.LastSoilMoisture = soilMoistureReading.Value;
                _logger.LogInformation($"Recorded soil moisture {soilMoistureReading.Value}% after watering.");
            }

            // Optionally, update the status immediately after watering.
            // This prevents the user from seeing "NeedsWatering" right after they watered.
            // Recalculate status based on new state (watering counter = 0, potentially high soil moisture)
            // The logic below is similar to UpdatePlantStatusAsync but only considers manual watering impact.
            StatusEnum newStatusAfterWatering = StatusEnum.Normal;
            string? newStatusReason = null;

            // After watering, the plant is typically Normal unless there's an extreme condition not resolved by watering
            // (like extreme temp/humidity, or *excessive* soil moisture from overwatering if sensor data is available)

            if (gardenPlant.LastSoilMoisture > 0 && gardenPlant.LastSoilMoisture > gardenPlant.Plant.MaxSoilMoisture)
            {
                newStatusAfterWatering = StatusEnum.AtRisk;
                newStatusReason = $"Soil moisture ({gardenPlant.LastSoilMoisture}%) is above the maximum threshold ({gardenPlant.Plant.MaxSoilMoisture}%) even after watering (potential overwatering).";
                _logger.LogWarning($"Plant {gardenPlant.Plant?.Name ?? "Unknown"} (ID: {gardenPlant.Id}) AtRisk after watering: {newStatusReason}");
            }
            else
            {
                newStatusAfterWatering = StatusEnum.Normal; // Watering usually resolves NeedsWatering
                newStatusReason = null;
                _logger.LogInformation($"Plant {gardenPlant.Plant?.Name ?? "Unknown"} (ID: {gardenPlant.Id}): Status set to Normal after watering.");
            }


            gardenPlant.Status = newStatusAfterWatering;
            gardenPlant.StatusChangeReason = newStatusReason;
            gardenPlant.LastStatusCheckDate = DateTime.UtcNow; // Record when status was last updated

            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully recorded watering for GardenPlant ID: {gardenPlantId}. Counter reset. Status updated to {gardenPlant.Status}.");
                return true;
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, $"Failed to save watering record for GardenPlant ID: {gardenPlantId}.");
                throw; // Re-throw or handle appropriately
            }
        }

        // You could add other methods here for other user actions (e.g., Fertilizing, Pruning)
        // which might update LastFertilizedDate, DaysSinceLastPruned, etc.,
        // and influence other potential status checks (NeedsFertilization, NeedsPruning).
    }
}