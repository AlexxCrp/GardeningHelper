using AutoMapper;
using DataExchange.DTOs.Response;
using GardeningHelperDatabase.Entities;

namespace GardeningHelperDatabase.Mappings
{
    public class GardenProfile : Profile
    {
        public GardenProfile()
        {
            CreateMap<UserGarden, UserGardenResponseDTO>();
            CreateMap<UserGardenResponseDTO, UserGarden>();
            CreateMap<GardenPlant, GardenPlantResponseDTO>();
            CreateMap<GardenPlantResponseDTO, GardenPlant>();
        }
    }
}
