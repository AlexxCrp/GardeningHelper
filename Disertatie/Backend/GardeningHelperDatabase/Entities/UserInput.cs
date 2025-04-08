using GardeningHelperDatabase.Entities.Identity;

namespace GardeningHelperDatabase.Entities
{
    public class UserInput
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to the User table
        public User User { get; set; } // Navigation property to the User

        public int PlantId { get; set; } // Foreign key to the Plant table
        public Plant Plant { get; set; } // Navigation property to the Plant

        public double SoilMoisture { get; set; } // Soil moisture percentage
        public string GrowthStage { get; set; } // Growth stage of the plant
        public string Observations { get; set; } // User observations
        public DateTime InputDate { get; set; } // Date of the input
    }
}
