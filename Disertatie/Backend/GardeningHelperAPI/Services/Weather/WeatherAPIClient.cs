using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GardeningHelperAPI.Services.Weather
{
    public class WeatherAPIClient
    {
        private readonly HttpClient _httpClient;
        private readonly OpenWeatherMapSettings _settings;
        private readonly ILogger<WeatherAPIClient> _logger; // Optional

        public WeatherAPIClient(HttpClient httpClient, IOptions<OpenWeatherMapSettings> settings, ILogger<WeatherAPIClient> logger = null)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger; // Inject logger if configured
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<OpenWeatherMapCurrentResponse> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            // Use the 'metric' unit for temperature in Celsius and rain/snow in mm
            var requestUrl = $"{_settings.BaseUrl}weather?lat={latitude}&lon={longitude}&appid={_settings.ApiKey}&units=metric";

            try
            {
                _logger?.LogInformation($"Fetching weather data from: {requestUrl}");
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws an exception for non-success status codes (4xx or 5xx)

                var content = await response.Content.ReadAsStringAsync();
                _logger?.LogDebug($"OpenWeatherMap API response: {content}");

                var weatherData = JsonSerializer.Deserialize<OpenWeatherMapCurrentResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Allow case-insensitive matching of JSON property names
                });

                return weatherData;
            }
            catch (HttpRequestException httpEx)
            {
                _logger?.LogError(httpEx, $"HTTP request failed when fetching weather for lat={latitude}, lon={longitude}: {httpEx.Message}");
                // Handle specific HTTP errors if needed (e.g., API key invalid, location not found)
                return null; // Or throw a custom exception
            }
            catch (JsonException jsonEx)
            {
                _logger?.LogError(jsonEx, $"JSON deserialization failed when fetching weather for lat={latitude}, lon={longitude}: {jsonEx.Message}");
                return null; // Or throw a custom exception
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"An unexpected error occurred when fetching weather for lat={latitude}, lon={longitude}: {ex.Message}");
                return null; // Or throw a custom exception
            }
        }
    }
}
