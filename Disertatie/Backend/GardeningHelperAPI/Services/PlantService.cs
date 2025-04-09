namespace GardeningHelperAPI.Services
{
    using AutoMapper;
    using DataExchange.DTOs.Response;
    using GardeningHelperDatabase;
    using GardeningHelperDatabase.Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PlantService
    {
        private readonly GardeningHelperDbContext _dbContext;
        private readonly IMapper _mapper;

        public PlantService(GardeningHelperDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // Get all plants
        public async Task<List<PlantDTO>> GetAllPlantsAsync()
        {
            var plants = await _dbContext.Plants.ToListAsync();
            return _mapper.Map<List<PlantDTO>>(plants);
        }

        public async Task<List<PlantCardDTO>> GetPlantCards()
        {
            var plants = await _dbContext.Plants.ToListAsync();
            return _mapper.Map<List<PlantCardDTO>>(plants);
        }

        // Get a plant by ID
        public async Task<PlantDTO> GetPlantByIdAsync(int id)
        {
            var plant = await _dbContext.Plants.FindAsync(id);
            return _mapper.Map<PlantDTO>(plant);
        }

        // Create a new plant
        public async Task<PlantDTO> CreatePlantAsync(PlantDTO createPlantDto)
        {
            var plant = _mapper.Map<Plant>(createPlantDto);
            _dbContext.Plants.Add(plant);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<PlantDTO>(plant);
        }

        // Update an existing plant
        public async Task<PlantDTO> UpdatePlantAsync(int id, PlantDTO updatePlantDto)
        {
            var plant = await _dbContext.Plants.FindAsync(id);
            if (plant == null)
            {
                return null; // Plant not found
            }

            _mapper.Map(updatePlantDto, plant);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<PlantDTO>(plant);
        }

        // Delete a plant by ID
        public async Task<bool> DeletePlantAsync(int id)
        {
            var plant = await _dbContext.Plants.FindAsync(id);
            if (plant == null)
            {
                return false; // Plant not found
            }

            _dbContext.Plants.Remove(plant);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
