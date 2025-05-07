namespace DataExchange.DTOs.Response
{
    public class UserGardenResponseDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int XSize { get; set; }
        public int YSize { get; set; }

        public List<GardenPlantResponseDTO> GardenPlants { get; set; } = new List<GardenPlantResponseDTO>();
    }
}
