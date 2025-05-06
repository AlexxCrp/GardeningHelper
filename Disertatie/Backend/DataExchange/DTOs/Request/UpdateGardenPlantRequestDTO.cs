namespace DataExchange.DTOs.Request
{
    public class UpdateGardenPlantRequestDTO
    {
        public int GardenPlantId { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public DateTime LastWateredDate { get; set; }
        public double LastSoilMoisture { get; set; }
    }
}
