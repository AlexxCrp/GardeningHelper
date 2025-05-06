// Backend/Services/GardenService.cs
using AutoMapper;
using DataExchange.DTOs.Request;
using DataExchange.DTOs.Response;
using GardeningHelperAPI.Services;
using GardeningHelperDatabase;
using GardeningHelperDatabase.Entities;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class GardenService
    {
        private readonly GardeningHelperDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly PlantService _plantService;
        private readonly IMapper _mapper;

        public GardenService(GardeningHelperDbContext dbContext, UserManager<User> userManager, PlantService plantService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
            _plantService = plantService;
        }

        public async Task<UserGardenResponseDTO> GetUserGardenAsync(string userName)
        {
            var garden = await _dbContext.UserGardens
                .Include(g => g.GardenPlants)
                    .ThenInclude(gp => gp.Plant)
                .FirstOrDefaultAsync(g => g.User.UserName == userName);

            if (garden == null)
                return null;

            var response = _mapper.Map<UserGardenResponseDTO>(garden);
            foreach (var gardenPlant in response.GardenPlants)
            {
                var plant = await _plantService.GetPlantByNameAsync(gardenPlant.PlantName);
                gardenPlant.Base64Image = plant.ImageBase64;
            }

            return response;
        }

        public async Task<UserGardenResponseDTO> CreateGardenAsync(string userName, CreateGardenRequestDTO request)
        {
            // Check if user already has a garden
            var existingGarden = await _dbContext.UserGardens
                .FirstOrDefaultAsync(g => g.User.UserName == userName);

            if (existingGarden != null)
                throw new InvalidOperationException("User already has a garden");

            User user = await _userManager.FindByNameAsync(userName);

            var garden = new UserGarden
            {
                UserId = user.Id,
                xSize = request.XSize,
                ySize = request.YSize,
                GardenPlants = new List<GardenPlant>()
            };

            _dbContext.UserGardens.Add(garden);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserGardenResponseDTO>(garden);
        }

        public async Task<UserGardenResponseDTO> UpdateGardenAsync(string userName, CreateGardenRequestDTO request)
        {
            var existingGarden = await _dbContext.UserGardens
                .Include(x => x.GardenPlants)
                .FirstOrDefaultAsync(g => g.User.UserName == userName);

            existingGarden.xSize = request.XSize;
            existingGarden.ySize = request.YSize;

            var plantsToDelete = existingGarden.GardenPlants.Where(x => x.PositionX >= existingGarden.xSize || x.PositionY >= existingGarden.ySize).ToList();
            if (plantsToDelete.Any())
            {
                var gardenPlants = existingGarden.GardenPlants.ToList();
                gardenPlants.RemoveAll(x => x.PositionX >= existingGarden.xSize || x.PositionY >= existingGarden.ySize);
                existingGarden.GardenPlants = gardenPlants;
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserGardenResponseDTO>(existingGarden);
        }

        public async Task<UserGardenResponseDTO> UpdateGardenPlantAsync(string userName, UpdateGardenPlantRequestDTO request)
        {
            var existingGarden = await _dbContext.UserGardens
                .Include(x => x.GardenPlants)
                    .ThenInclude(x => x.Plant)
                .FirstOrDefaultAsync(g => g.User.UserName == userName);

            var plantToUpdate = existingGarden.GardenPlants.FirstOrDefault(x => x.Id == request.GardenPlantId);

            plantToUpdate.PositionX = request.PositionX;
            plantToUpdate.PositionY = request.PositionY;
            plantToUpdate.LastSoilMoisture = request.LastSoilMoisture;
            plantToUpdate.LastWateredDate = request.LastWateredDate;

            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<UserGardenResponseDTO>(existingGarden);

            foreach (var gardenPlant in response.GardenPlants)
            {
                var plant = await _plantService.GetPlantByNameAsync(gardenPlant.PlantName);
                gardenPlant.Base64Image = plant.ImageBase64;
            }

            return response;
        }

        public async Task<GardenPlantResponseDTO> AddPlantToGardenAsync(string userName, AddPlantToGardenRequestDTO request)
        {
            var garden = await _dbContext.UserGardens
                .FirstOrDefaultAsync(g => g.User.UserName == userName);

            if (garden == null)
                throw new InvalidOperationException("User does not have a garden");

            // Check if position is valid
            if (request.PositionX < 0 || request.PositionX >= garden.xSize ||
                request.PositionY < 0 || request.PositionY >= garden.ySize)
                throw new InvalidOperationException("Invalid position");

            // Check if position is already occupied
            var existingPlant = await _dbContext.GardenPlants
                .FirstOrDefaultAsync(gp => gp.UserGardenId == garden.Id &&
                                          gp.PositionX == request.PositionX &&
                                          gp.PositionY == request.PositionY);

            if (existingPlant != null)
                throw new InvalidOperationException("Position is already occupied");

            var plant = await _dbContext.Plants.FindAsync(request.PlantId);
            if (plant == null)
                throw new InvalidOperationException("Plant not found");

            var gardenPlant = new GardenPlant
            {
                UserGardenId = garden.Id,
                PlantId = request.PlantId,
                Plant = plant,
                PositionX = request.PositionX,
                PositionY = request.PositionY,
                LastWateredDate = DateTime.UtcNow,
                LastStatusCheckDate = DateTime.UtcNow,
                DaysToWateringCounter = 0,
                LastSoilMoisture = plant.MinSoilMoisture + (plant.MaxSoilMoisture - plant.MinSoilMoisture) / 2 // Initial value as average
            };

            _dbContext.GardenPlants.Add(gardenPlant);
            await _dbContext.SaveChangesAsync();

            // Reload the garden plant with the plant information
            gardenPlant = await _dbContext.GardenPlants
                .Include(gp => gp.Plant)
                .FirstAsync(gp => gp.Id == gardenPlant.Id);

            GardenPlantResponseDTO response = _mapper.Map<GardenPlantResponseDTO>(gardenPlant);
            response.Base64Image = $"data:image/png;base64,{Convert.ToBase64String(plant.Image)}";
            return response;
        }

        public async Task<bool> RemovePlantFromGardenAsync(string userName, int gardenPlantId)
        {
            var garden = await _dbContext.UserGardens
                .FirstOrDefaultAsync(g => g.User.UserName == userName);

            if (garden == null)
                return false;

            var gardenPlant = await _dbContext.GardenPlants
                .FirstOrDefaultAsync(gp => gp.Id == gardenPlantId && gp.UserGardenId == garden.Id);

            if (gardenPlant == null)
                return false;

            _dbContext.GardenPlants.Remove(gardenPlant);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
