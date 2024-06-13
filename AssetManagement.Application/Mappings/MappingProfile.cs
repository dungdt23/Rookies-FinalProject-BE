using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Entities;
using AutoMapper;

namespace AssetManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // Mapping User
            CreateMap<CreateUserForm, User>();
            CreateMap<User, ResponseUserDto>();
        }
    }
}
