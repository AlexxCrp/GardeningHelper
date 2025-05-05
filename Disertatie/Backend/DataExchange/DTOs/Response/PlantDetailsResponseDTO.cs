using DataExchange.Enums;

namespace DataExchange.DTOs.Response
{
    public class PlantDetailsResponseDTO
    {
        public int Id { get; set; }
        public int PlantId { get; set; }

        public BloomSeasonEnum BloomSeason { get; set; }
        public PlantLifecycleEnum Lifecycle { get; set; }
        public WaterNeedsEnum WaterNeeds { get; set; }
        public DifficultyLevelEnum DifficultyLevel { get; set; }

        public string NativeTo { get; set; }
        public string IdealPhLevel { get; set; }
        public string GrowingZones { get; set; }
        public string HeightAtMaturity { get; set; }
        public string SpreadAtMaturity { get; set; }
        public string DaysToGermination { get; set; }
        public string DaysToMaturity { get; set; }
        public string PlantingDepth { get; set; }
        public string SpacingBetweenPlants { get; set; }

        public List<PlantPurposeEnum> Purposes { get; set; }

        public string PropagationMethods { get; set; }
        public string PruningInstructions { get; set; }
        public string PestManagement { get; set; }
        public string DiseaseManagement { get; set; }
        public string FertilizationSchedule { get; set; }
        public string WinterCare { get; set; }
        public string HarvestingTips { get; set; }
        public string StorageTips { get; set; }

        public string CulinaryUses { get; set; }
        public string MedicinalUses { get; set; }
        public string HistoricalNotes { get; set; }
        public string AdditionalNotes { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<int> CompanionPlantIds { get; set; } = new List<int>();
    }
}
