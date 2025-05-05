using DataExchange.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GardeningHelperDatabase.Entities
{
    public class PlantDetails
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Plant")]
        public int PlantId { get; set; }

        // Navigation property to the parent Plant
        public Plant Plant { get; set; }

        // Detailed plant properties
        public BloomSeasonEnum BloomSeason { get; set; }
        public PlantLifecycleEnum Lifecycle { get; set; }
        public WaterNeedsEnum WaterNeeds { get; set; }
        public DifficultyLevelEnum DifficultyLevel { get; set; }

        // Specific growing details
        public string NativeTo { get; set; }
        public string IdealPhLevel { get; set; }
        public string GrowingZones { get; set; }
        public string HeightAtMaturity { get; set; }
        public string SpreadAtMaturity { get; set; }
        public string DaysToGermination { get; set; }
        public string DaysToMaturity { get; set; }
        public string PlantingDepth { get; set; }
        public string SpacingBetweenPlants { get; set; }

        // Multiple purposes that a plant can serve
        public List<PlantPurposeEnum> Purposes { get; set; }

        // Detailed care instructions
        public string PropagationMethods { get; set; }
        public string PruningInstructions { get; set; }
        public string PestManagement { get; set; }
        public string DiseaseManagement { get; set; }
        public string FertilizationSchedule { get; set; }
        public string WinterCare { get; set; }
        public string HarvestingTips { get; set; }
        public string StorageTips { get; set; }

        // Additional information
        public string CulinaryUses { get; set; }
        public string MedicinalUses { get; set; }
        public string HistoricalNotes { get; set; }
        public string AdditionalNotes { get; set; }

        // Collection of image URLs
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Companion planting
        public List<int> CompanionPlantIds { get; set; } = new List<int>();

        [NotMapped]
        public List<Plant> CompanionPlants { get; set; } = new List<Plant>();
    }
}
