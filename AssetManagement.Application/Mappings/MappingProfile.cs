﻿using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Domain.Entities;
using AutoMapper;

namespace AssetManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // Mapping User
            CreateMap<RequestUserCreateDto, User>()
                .ForMember(dest => dest.Type, opt => opt.Ignore());
            CreateMap<RequestUserEditDto, User>()
                .ForMember(dest => dest.Type, opt => opt.Ignore());
            CreateMap<User, ResponseUserDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.TypeName));
            //Mapping Category
            CreateMap<Category, ResponseCategoryDto>();
            CreateMap<RequestCategoryDto, Category>();
            CreateMap<Category, ResponseReportDto>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Assets.Where(x => !x.IsDeleted).Count()))
                .ForMember(dest => dest.Assigned, opt => opt.MapFrom(src => src.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.Assigned).Count()))
                .ForMember(dest => dest.Available, opt => opt.MapFrom(src => src.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.Available).Count()))
                .ForMember(dest => dest.NotAvailable, opt => opt.MapFrom(src => src.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.NotAvailable).Count()))
                .ForMember(dest => dest.WaitingForRecycling, opt => opt.MapFrom(src => src.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.WaitingForRecycling).Count()))
                .ForMember(dest => dest.Recycled, opt => opt.MapFrom(src => src.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.Recycled).Count()));

            //Mapping Type
            CreateMap<Domain.Entities.Type, ResponseTypeDto>();
            //Mapping Location
            CreateMap<Location, ResponseLocationDto>();
            //Mapping Asset
            CreateMap<Asset, ResponseAssetDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.CategoryName));
            CreateMap<RequestAssetDto, Asset>();

            //Mapping Assignment
            CreateMap<RequestAssignmentDto, Assignment>();
            CreateMap<Assignment, ResponseAssignmentDto>()
                .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.Assigner.UserName))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.Assignee.UserName))
                .ForMember(dest => dest.AssetCode, opt => opt.MapFrom(src => src.Asset.AssetCode))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.AssetName))
                .ForMember(dest => dest.Specification, opt => opt.MapFrom(src => src.Asset.Specification));
            CreateMap<Assignment, ResponseHistoryAsmDto>()
                 .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.Assigner.UserName))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.Assignee.UserName))
                .ForMember(dest => dest.AssetCode, opt => opt.MapFrom(src => src.Asset.AssetCode))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.AssetName))
                .ForMember(dest => dest.Specification, opt => opt.MapFrom(src => src.Asset.Specification))
                .ForMember(dest => dest.ReturnRequest, opt => opt.MapFrom(src => src.ReturnRequests.FirstOrDefault(x => x.Id == src.ActiveReturnRequestId)));

            //Mapping Return Request
            CreateMap<ReturnRequest, ResponseReturnRequestGetAllDto>()
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.Assignment.Asset.Id))
                .ForMember(dest => dest.AssetCode, opt => opt.MapFrom(src => src.Assignment.Asset.AssetCode))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Assignment.Asset.AssetName))
                .ForMember(dest => dest.RequestorUsername, opt => opt.MapFrom(src => src.Requestor.UserName))
                .ForMember(dest => dest.ResponderUsername, opt => opt.MapFrom(src => src.Responder.UserName))
                .ForMember(dest => dest.AssignmentAssignedDate, opt => opt.MapFrom(src => src.Assignment.AssignedDate));
            CreateMap<ReturnRequest, ResponseReturnRequestDto>();
            CreateMap<ReturnRequest, ResponseHistoryReturnRequestDto>();
        }
    }
}
