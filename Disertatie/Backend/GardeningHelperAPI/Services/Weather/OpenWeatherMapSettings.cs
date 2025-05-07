namespace GardeningHelperAPI.Services.Weather
{
    public class OpenWeatherMapSettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; } = "https://api.openweathermap.org/data/2.5/";
    }
}
