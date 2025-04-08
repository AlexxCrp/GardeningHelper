using GardeningHelperDatabase.Entities.Identity;

namespace GardeningHelperDatabase.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to the User table
        public User User { get; set; } // Navigation property to the User

        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
    }
}
