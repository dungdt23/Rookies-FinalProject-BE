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
            CreateMap<CreateUpdateUserForm, User>();
            CreateMap<User, ResponseUserDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.TypeName));
            //Mapping Category
            CreateMap<Category, ResponseCategoryDto>();
            CreateMap<Domain.Entities.Type, ResponseTypeDto>();
            CreateMap<Location, ResponseLocationDto>();

        }
    }
}
