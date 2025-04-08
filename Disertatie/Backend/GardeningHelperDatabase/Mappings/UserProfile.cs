using AutoMapper;
using DataExchange.DTOs.Request;
using GardeningHelperDatabase.Entities.Identity;

namespace GardeningHelperDatabase.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterRequestDTO, User>();
        }
    }
}
