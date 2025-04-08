using DataExchange.Enums;

namespace GardeningHelperDatabase.Entities
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CareInstructions { get; set; }
        public SunlightRequirementsEnum SunlightRequirements { get; set; }
        public SoilTypeEnum SoilType { get; set; }
        public string GrowthPeriod { get; set; }
        public string HarvestTime { get; set; }
        public byte[] Image { get; set; }

        // Thresholds for care notifications
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double MinHumidity { get; set; }
        public double MaxHumidity { get; set; }
        public double MinRainfall { get; set; }
        public double MaxRainfall { get; set; }
        public double MinSoilMoisture { get; set; }
        public double MaxSoilMoisture { get; set; }

        // Watering thresholds
        public int WateringThresholdDays { get; set; }
        public double WateringThresholdRainfall { get; set; }

        // Status of the plant (calculated)
        public StatusEnum Status { get; set; }

        // Navigation property to GardenPlant
        public ICollection<GardenPlant> GardenPlants { get; set; }

        // Navigation property to UserInput
        public ICollection<UserInput> UserInputs { get; set; } = new List<UserInput>();
    }
}
