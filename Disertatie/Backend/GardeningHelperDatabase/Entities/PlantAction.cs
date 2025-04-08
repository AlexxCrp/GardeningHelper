using DataExchange.Enums;

namespace GardeningHelperDatabase.Entities
{
    public class PlantAction
    {
        public int Id { get; set; }
        public string Name { get; set; } // Name of the action (e.g., "Water the plant")
        public string Description { get; set; } // Detailed description of the action
        public StatusEnum TriggerStatus { get; set; } // Status that triggers this action
    }
}
