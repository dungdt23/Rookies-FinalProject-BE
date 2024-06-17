﻿using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.IAssetServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services.AssetServices
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        public AssetService(IAssetRepository assetRepository,IGenericRepository<Category> categoryRepository,IMapper mapper)
        {
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;   
            _mapper = mapper;
        }
        public async Task<PagedResponse<ResponseAssetDto>> GetAllAsync(AssetFilter filter, int? index, int? size)
        {
            Func<Asset, object> sortConditon = x => x.AssetCode;
            switch (filter.sort)
            {
                case AssetSort.AssetName:
                    sortConditon = x => x.AssetCode;
                    break;
                case AssetSort.Category:
                    sortConditon = x => x.Category.CategoryName;
                    break;
                case AssetSort.State:
                    sortConditon = x => nameof(x.State) == nameof(AssetSort.State);
                    break;
            }
            var assets = await _assetRepository.GetAllAsync(sortConditon, filter, index, size);
            var assetDtos = _mapper.Map<IEnumerable<ResponseAssetDto>>(assets);
            var totalCount = await _assetRepository.GetTotalCountAsync(filter);
            return new PagedResponse<ResponseAssetDto>
            {
                Data = assetDtos,
                TotalCount = totalCount,
                Message = (assetDtos.Count() != 0) ? "Get asset list successfully!" : "List asset is empty",
            };
        }
        public async Task<ApiResponse> AddAsync(RequestAssetDto requestAssetDto)
        {
            var asset = _mapper.Map<Asset>(requestAssetDto);
            var category = await _categoryRepository.GetByCondition(x => x.Id == requestAssetDto.CategoryId)
                .FirstOrDefaultAsync();
            var assetCode = await _assetRepository.CreateAssetCode(category.Prefix);
            asset.AssetCode = assetCode;
            var status = await _assetRepository.AddAsync(asset);
            if (status == StatusConstant.Success)
            {
                return new ApiResponse
                {
                    Data = requestAssetDto,
                    Message = "Add new asset successfully"
                };
            }
            else
            {
                return new ApiResponse
                {
                    Message = "Add new asset failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ApiResponse> UpdateAsync(Guid id, RequestAssetDto requestAssetDto)
        {
            var asset = await _assetRepository.GetByCondition(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (asset == null)
            {
                return new ApiResponse
                {
                    Message = "Asset doesn't exist",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            if (asset.State == Domain.Enums.TypeAssetState.Assigned)
            {
                return new ApiResponse
                {
                    Message = "Can't update asset because it is assigned",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            var updateAsset = _mapper.Map<Asset>(requestAssetDto);
            updateAsset.Id = asset.Id;
            var status = await _assetRepository.UpdateAsync(updateAsset);
            if (status == StatusConstant.Success)
            {
                return new ApiResponse
                {
                    Data = requestAssetDto,
                    Message = "Update asset successfully"
                };
            }
            else
            {
                return new ApiResponse
                {
                    Message = "Update asset failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            var asset = await _assetRepository.GetByCondition(x => x.Id == id)
                .Include(x => x.Assignments)
                .FirstOrDefaultAsync();
            if (asset == null)
            {
                return new ApiResponse
                {
                    Message = "Asset doesn't exist",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            //check if asset belong to any historical assignment
            var historicalAssignment = asset.Assignments.FirstOrDefault(x => !x.IsDeleted);
            if (historicalAssignment != null) return new ApiResponse
            {
                Message = "Can not be deleted! Asset belong to an historical assignment",
                StatusCode = StatusCodes.Status500InternalServerError
            };
            //check if asset is available
            if (asset.State != Domain.Enums.TypeAssetState.Available
             || asset.State != Domain.Enums.TypeAssetState.Assigned)
            {
                return new ApiResponse
                {
                    Message = "Asset not available! Please update its state",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            else
            {
                var status = await _assetRepository.DeleteAsync(id);
                if (status == StatusConstant.Success)
                {
                    return new ApiResponse
                    {
                        Message = "Delete asset successfully"
                    };
                }
                else
                {
                    return new ApiResponse
                    {
                        Message = "Delete asset failed",
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }
    }
}