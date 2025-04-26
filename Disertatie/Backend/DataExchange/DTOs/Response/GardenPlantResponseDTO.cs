namespace DataExchange.DTOs.Response
{
    public class GardenPlantResponseDTO
    {
        public int Id { get; set; }
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string Status { get; set; }
        public DateTime LastWateredDate { get; set; }
        public double LastSoilMoisture { get; set; }
        public string Base64Image { get; set; }
    }
}
