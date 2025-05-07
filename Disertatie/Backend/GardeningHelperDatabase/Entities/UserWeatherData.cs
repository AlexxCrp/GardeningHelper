using GardeningHelperDatabase.Entities.Identity;

namespace GardeningHelperDatabase.Entities
{
    public class UserWeatherData
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to the User table
        public User User { get; set; } // Navigation property to the User

        public DateTime Date { get; set; }
        public string General { get; set; }
        public double Temperature { get; set; } // Temperature in Celsius
        public double Humidity { get; set; } // Humidity percentage
        public double Rainfall { get; set; } // Rainfall in mm
    }
}
