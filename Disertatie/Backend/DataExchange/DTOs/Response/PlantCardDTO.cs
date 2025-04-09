namespace DataExchange.DTOs.Response
{
    public class PlantCardDTO
    {
        public string Name { get; set; }
        public byte[] Image { get; set; }

        public string ImageBase64 => Image != null
            ? $"data:image/png;base64,{Convert.ToBase64String(Image)}"
            : null;
    }
}
