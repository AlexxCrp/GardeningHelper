using Microsoft.AspNetCore.Identity;

namespace GardeningHelperDatabase.Entities.Identity
{
    public class User : IdentityUser
    {
        public ICollection<UserGarden> UserGardens { get; set; }
        public ICollection<UserInput> UserInputs { get; set; }
        public ICollection<UserWeatherData> UserWeatherData { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
