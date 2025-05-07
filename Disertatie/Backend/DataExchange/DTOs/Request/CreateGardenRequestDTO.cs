namespace DataExchange.DTOs.Request
{
    public class CreateGardenRequestDTO
    {
        public int XSize { get; set; }
        public int YSize { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
