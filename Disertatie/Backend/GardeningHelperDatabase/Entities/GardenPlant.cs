using DataExchange.Enums;

namespace GardeningHelperDatabase.Entities
{
    public class GardenPlant
    {
        public int Id { get; set; }

        // Foreign keys
        public int UserGardenId { get; set; } // Foreign key to the UserGarden table
        public UserGarden UserGarden { get; set; } // Navigation property to the UserGarden

        public int PlantId { get; set; } // Foreign key to the Plant table
        public Plant Plant { get; set; } // Navigation property to the Plant

        // Plant-specific data
        public int PositionX { get; set; } // X position in the garden grid
        public int PositionY { get; set; } // Y position in the garden grid
        public int DaysToWateringCounter { get; set; } // Days since last watering
        public DateTime LastWateredDate { get; set; } // Date when the plant was last watered
        public DateTime LastRainfallDate { get; set; } // Date when it last rained
        public double LastRainfallAmount { get; set; } // Amount of rainfall in mm
        public double LastSoilMoisture { get; set; } // Last recorded soil moisture percentage
        public DateTime LastStatusCheckDate { get; set; } // Date when the status was last checked
        public StatusEnum Status { get; set; }        // Status of the plant (calculated)
    }
}
