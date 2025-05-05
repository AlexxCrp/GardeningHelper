using AutoMapper;
using DataExchange.DTOs.Response;
using GardeningHelperDatabase.Entities;

namespace GardeningHelperDatabase.Mappings
{
    public class PlantProfile : Profile
    {
        public PlantProfile()
        {
            CreateMap<Plant, PlantDTO>();
            CreateMap<PlantDTO, Plant>();

            CreateMap<Plant, PlantCardDTO>();
            CreateMap<PlantCardDTO, Plant>();

            CreateMap<PlantDetails, PlantDetailsResponseDTO>();
            CreateMap<PlantDetailsResponseDTO, PlantDetails>();
        }
    }
}
