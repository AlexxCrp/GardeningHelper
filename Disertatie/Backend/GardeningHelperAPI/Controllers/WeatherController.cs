using GardeningHelperAPI.Services.Weather;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GardeningHelperAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly UserManager<User> _userManager; // Needed to get UserId

        public WeatherController(WeatherService weatherService, UserManager<User> userManager)
        {
            _weatherService = weatherService;
            _userManager = userManager;
        }

        [HttpPost("fetch-current")]
        public async Task<IActionResult> FetchCurrentWeatherForUser()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Or handle appropriately if user is not logged in
            }

            bool success = await _weatherService.FetchAndSaveCurrentWeatherForUserAsync(userId);

            if (success)
            {
                return Ok("Weather data fetched and saved successfully.");
            }
            else
            {
                // Return a more specific error if possible, or a generic error
                return StatusCode(500, "Failed to fetch or save weather data.");
            }
        }
    }
}
