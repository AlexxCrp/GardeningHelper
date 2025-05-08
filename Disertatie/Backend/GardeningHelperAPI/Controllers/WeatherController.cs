using GardeningHelperAPI.Services;
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
        private readonly UserManager<User> _userManager;
        private readonly GardenBackgroundService _backgroundService;

        // Inject the background service instance
        public WeatherController(WeatherService weatherService, UserManager<User> userManager, GardenBackgroundService backgroundService, ILogger<WeatherController> logger = null)
        {
            _weatherService = weatherService;
            _userManager = userManager;
            _backgroundService = backgroundService;
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

        //[HttpPost("trigger-all-users-update")]
        //// CONSIDER: Add authorization here! Only administrators should likely trigger this manually.
        //public async Task<IActionResult> TriggerAllUsersWeatherUpdate()
        //{
        //    try
        //    {
        //        await _backgroundService.TriggerWeatherUpdateForAllUsersAsync();
        //        //Add here Status Method call
        //        //Add here notification service call
        //        return Ok("Weather update process for all users has been triggered.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
