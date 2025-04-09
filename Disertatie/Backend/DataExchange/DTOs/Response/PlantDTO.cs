
using DataExchange.Enums;

namespace DataExchange.DTOs.Response
{
    public class PlantDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CareInstructions { get; set; }
        public SunlightRequirementsEnum SunlightRequirements { get; set; }
        public SoilTypeEnum SoilType { get; set; }
        public string GrowthPeriod { get; set; }
        public HarvestTimeEnum HarvestTime { get; set; }
        public byte[] Image { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double MinHumidity { get; set; }
        public double MaxHumidity { get; set; }
        public double MinRainfall { get; set; }
        public double MaxRainfall { get; set; }
        public double MinSoilMoisture { get; set; }
        public double MaxSoilMoisture { get; set; }
        public int WateringThresholdDays { get; set; }
        public double WateringThresholdRainfall { get; set; }
        public StatusEnum Status { get; set; }
    }
}
