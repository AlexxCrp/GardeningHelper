using System.Text.Json.Serialization;

namespace GardeningHelperAPI.Services.Weather
{
    // Root object for the current weather response
    public class OpenWeatherMapCurrentResponse
    {
        [JsonPropertyName("coord")]
        public Coordinates Coord { get; set; }

        [JsonPropertyName("weather")]
        public WeatherDescription[] Weather { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("main")]
        public MainWeatherInfo Main { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("wind")]
        public WindInfo Wind { get; set; }

        [JsonPropertyName("clouds")]
        public CloudsInfo Clouds { get; set; }

        [JsonPropertyName("dt")]
        public long Dt { get; set; } // Time of data calculation, unix, UTC

        [JsonPropertyName("sys")]
        public SystemInfo Sys { get; set; }

        [JsonPropertyName("timezone")]
        public int Timezone { get; set; } // Shift in seconds from UTC

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } // City name

        [JsonPropertyName("cod")]
        public int Cod { get; set; } // Internal parameter

        // Rain volume for the last 1 hour (optional)
        [JsonPropertyName("rain")]
        public RainInfo Rain { get; set; }

        // Snow volume for the last 1 hour (optional)
        [JsonPropertyName("snow")]
        public SnowInfo Snow { get; set; }
    }

    public class Coordinates
    {
        [JsonPropertyName("lon")]
        public double Lon { get; set; } // Longitude

        [JsonPropertyName("lat")]
        public double Lat { get; set; } // Latitude
    }

    public class WeatherDescription
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("main")]
        public string Main { get; set; } // Group of weather parameters (e.g., Rain, Snow, Extreme)

        [JsonPropertyName("description")]
        public string Description { get; set; } // Weather condition within the group

        [JsonPropertyName("icon")]
        public string Icon { get; set; } // Weather icon id
    }

    public class MainWeatherInfo
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; } // Temperature (default Kelvin, specify units=metric for Celsius)

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; } // Temperature feels like

        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; } // Minimum temperature at the moment

        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; } // Maximum temperature at the moment

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; } // Atmospheric pressure (hPa)

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; } // Humidity (%)

        [JsonPropertyName("sea_level")]
        public int? SeaLevel { get; set; } // Atmospheric pressure on the sea level (hPa)

        [JsonPropertyName("grnd_level")]
        public int? GrndLevel { get; set; } // Atmospheric pressure on the ground level (hPa)
    }

    public class WindInfo
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; } // Wind speed (default m/s, specify units=metric for m/s)

        [JsonPropertyName("deg")]
        public int Deg { get; set; } // Wind direction (degrees)

        [JsonPropertyName("gust")]
        public double? Gust { get; set; } // Wind gust (optional)
    }

    public class CloudsInfo
    {
        [JsonPropertyName("all")]
        public int All { get; set; } // Cloudiness (%)
    }

    public class SystemInfo
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; } // Sunrise time, unix, UTC

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; } // Sunset time, unix, UTC
    }

    public class RainInfo
    {
        [JsonPropertyName("1h")] // Note: JsonPropertyName is needed for property names starting with a number
        public double? D1h { get; set; } // Rain volume for the last 1 hour (mm)

        [JsonPropertyName("3h")]
        public double? D3h { get; set; } // Rain volume for the last 3 hours (mm)
    }

    public class SnowInfo
    {
        [JsonPropertyName("1h")]
        public double? S1h { get; set; } // Snow volume for the last 1 hour (mm)

        [JsonPropertyName("3h")]
        public double? S3h { get; set; } // Snow volume for the last 3 hours (mm)
    }
}
