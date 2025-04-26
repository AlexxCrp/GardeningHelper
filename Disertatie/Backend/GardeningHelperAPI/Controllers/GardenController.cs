// Backend/Controllers/GardenController.cs
using DataExchange.DTOs.Request;
using GardeningHelperAPI.Services;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;

namespace GardeningHelperAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GardenController : ControllerBase
    {
        private readonly GardenService _gardenService;
        private readonly PlantService _plantService;
        private readonly UserManager<User> _userManager;

        public GardenController(GardenService gardenService, UserManager<User> userManager, PlantService plantService)
        {
            _gardenService = gardenService;
            _plantService = plantService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGarden()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var garden = await _gardenService.GetUserGardenAsync(userName);
            if (garden == null)
                return NotFound(new { message = "Garden not found" });

            return Ok(garden);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGarden([FromBody] CreateGardenRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized();

                var garden = await _gardenService.CreateGardenAsync(userName, request);
                return Ok(garden);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the garden", error = ex.Message });
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateGarden([FromBody] CreateGardenRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized();

                var garden = await _gardenService.UpdateGardenAsync(userName, request);
                return Ok(garden);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the garden", error = ex.Message });
            }
        }

        [HttpPost("plants")]
        public async Task<IActionResult> AddPlantToGarden([FromBody] AddPlantToGardenRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized();

                var gardenPlant = await _gardenService.AddPlantToGardenAsync(userName, request);
                return Ok(gardenPlant);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the plant to the garden", error = ex.Message });
            }
        }

        [HttpDelete("plants/{gardenPlantId}")]
        public async Task<IActionResult> RemovePlantFromGarden(int gardenPlantId)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized();

                var result = await _gardenService.RemovePlantFromGardenAsync(userName, gardenPlantId);
                if (!result)
                    return NotFound(new { message = "Garden plant not found" });

                return Ok(new { message = "Plant removed from garden" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing the plant from the garden", error = ex.Message });
            }
        }
    }
}