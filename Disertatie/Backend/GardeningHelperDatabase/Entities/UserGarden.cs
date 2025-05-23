﻿using GardeningHelperDatabase.Entities.Identity;

namespace GardeningHelperDatabase.Entities
{
    public class UserGarden
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to the User table
        public User User { get; set; } // Navigation property to the User

        public int xSize { get; set; }
        public int ySize { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // List of plants in the garden
        public ICollection<GardenPlant> GardenPlants { get; set; }
    }
}
