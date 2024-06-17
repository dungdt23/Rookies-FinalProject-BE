using AssetManagement.Application.Dtos.RequestDtos;
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
            CreateMap<RequestCategoryDto, Category>();
            //Mapping Type
            CreateMap<Domain.Entities.Type, ResponseTypeDto>();
            //Mapping Location
            CreateMap<Location, ResponseLocationDto>();
            //Mapping Asset
            CreateMap<Asset, ResponseAssetDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()));

        }
    }
}
