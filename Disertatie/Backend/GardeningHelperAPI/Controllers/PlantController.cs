namespace GardeningHelperAPI.Controllers
{
    using DataExchange.DTOs.Response;
    using GardeningHelperAPI.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly PlantService _plantService;

        public PlantController(PlantService plantService)
        {
            _plantService = plantService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PlantDto>>> GetAllPlants()
        {
            var plants = await _plantService.GetAllPlantsAsync();
            return Ok(plants);
        }

        //[HttpGet]
        //public async Task<ActionResult<List<PlantDto>>> GetPlantCards()
        //{
        //    var plants = await _plantService.GetPlantCards();
        //    return Ok(plants);
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<PlantDto>> GetPlantById(int id)
        {
            var plant = await _plantService.GetPlantByIdAsync(id);
            if (plant == null)
            {
                return NotFound(); // Plant not found
            }
            return Ok(plant);
        }

        [HttpPost]
        public async Task<ActionResult<PlantDto>> CreatePlant([FromBody] PlantDto createPlantDto)
        {
            var createdPlant = await _plantService.CreatePlantAsync(createPlantDto);
            return CreatedAtAction(nameof(GetPlantById), new { id = createdPlant.Id }, createdPlant);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlantDto>> UpdatePlant(int id, [FromBody] PlantDto updatePlantDto)
        {
            var plant = await _plantService.UpdatePlantAsync(id, updatePlantDto);
            if (plant == null)
            {
                return NotFound(); // Plant not found
            }
            return Ok(plant);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeletePlant(int id)
        {
            var result = await _plantService.DeletePlantAsync(id);
            if (!result)
            {
                return NotFound(); // Plant not found
            }
            return Ok(result);
        }
    }
}
