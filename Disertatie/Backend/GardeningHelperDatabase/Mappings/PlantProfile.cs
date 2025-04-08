using AutoMapper;
using DataExchange.DTOs.Response;
using GardeningHelperDatabase.Entities;

namespace GardeningHelperDatabase.Mappings
{
    public class PlantProfile : Profile
    {
        public PlantProfile()
        {
            CreateMap<Plant, PlantDto>();
            CreateMap<PlantDto, Plant>();
        }
    }
}
