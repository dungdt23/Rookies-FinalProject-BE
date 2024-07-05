using AssetManagement.Application.ApiResponses;
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
        public AssetService(IAssetRepository assetRepository, IGenericRepository<Category> categoryRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<PagedResponse<ResponseAssetDto>> GetAllAsync(Guid locationId, AssetFilter filter, int? index, int? size)
        {
            Func<Asset, object> sortConditon = x => x.AssetCode;
            switch (filter.sort)
            {
                case AssetSort.AssetName:
                    sortConditon = x => x.AssetName;
                    break;
                case AssetSort.CategoryName:
                    sortConditon = x => x.Category.CategoryName;
                    break;
                case AssetSort.State:
                    sortConditon = x => x.State;
                    break;
            }
            var assets = await _assetRepository.GetAllAsync(sortConditon, locationId, filter, index, size);
            var assetDtos = _mapper.Map<IEnumerable<ResponseAssetDto>>(assets);
            var totalCount = await _assetRepository.GetTotalCountAsync(locationId, filter);
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
            var assetCode = _assetRepository.CreateAssetCode(category.Prefix, category.Id);
            asset.AssetCode = assetCode;
            var status = await _assetRepository.AddAsync(asset);
            if (status == StatusConstant.Success)
            {
                var returnAsset = _mapper.Map<ResponseAssetDto>(await _assetRepository.GetByCondition(a => a.Id == asset.Id)
                                                                                    .Include(a => a.Location)
                                                                                    .Include(a => a.Category)
                                                                                    .FirstOrDefaultAsync());
                return new ApiResponse
                {
                    Data = returnAsset,
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
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            if (asset.State == Domain.Enums.TypeAssetState.Assigned)
            {
                return new ApiResponse
                {
                    Message = "Can't update asset because it is assigned",
                    StatusCode = StatusCodes.Status409Conflict
                };
            }
            var updateAsset = _mapper.Map<Asset>(requestAssetDto);
            updateAsset.Id = asset.Id;
            updateAsset.AssetCode = asset.AssetCode;
            var status = await _assetRepository.UpdateAsync(updateAsset);
            if (status == StatusConstant.Success)
            {
                var responseAsset = _mapper.Map<ResponseAssetDto>(await _assetRepository.GetByCondition(a => a.Id == asset.Id)
                                                                                    .Include(a => a.Location)
                                                                                    .Include(a => a.Category)
                                                                                    .FirstOrDefaultAsync());
                return new ApiResponse
                {
                    Data = responseAsset,
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
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            //check if asset belong to any historical assignment
            var historicalAssignment = asset.Assignments.FirstOrDefault();
            if (historicalAssignment != null) return new ApiResponse
            {
                Message = "Can not be deleted! Asset belong to an historical assignment",
                StatusCode = StatusCodes.Status409Conflict
            };
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
        public async Task<ApiResponse> GetByIdAysnc(Guid id)
        {
            var asset = await _assetRepository
                .GetByCondition(x => x.Id == id)
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Location)
                .FirstOrDefaultAsync();
            var assetDto = _mapper.Map<ResponseAssetDto>(asset);
            return new ApiResponse
            {
                Data = assetDto
            };
        }
    }
}
