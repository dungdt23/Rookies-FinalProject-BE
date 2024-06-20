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
            CreateMap<CreateUpdateUserForm, User>()
                .ForMember(dest => dest.Type, opt => opt.Ignore());
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
            CreateMap<RequestAssetDto, Asset>();

            //Mapping Assignment
            CreateMap<RequestAssignmentDto, Assignment>();
            CreateMap<Assignment,ResponseAssignmentDto>()
                .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.Assigner.UserName))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.Assignee.UserName))
                .ForMember(dest => dest.AssetCode, opt => opt.MapFrom(src => src.Asset.AssetCode))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.AssetName))
                .ForMember(dest => dest.Specification, opt => opt.MapFrom(src => src.Asset.Specification))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()));


        }
    }
}
