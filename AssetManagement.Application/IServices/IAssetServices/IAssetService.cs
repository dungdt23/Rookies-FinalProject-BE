using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;

namespace AssetManagement.Application.IServices.IAssetServices
{
    public interface IAssetService
    {
        Task<PagedResponse<ResponseAssetDto>> GetAllAsync(Guid locationId, AssetFilter filter, int? index, int? size);
        Task<ApiResponse> AddAsync(RequestAssetDto requestAssetDto);
        Task<ApiResponse> UpdateAsync(Guid id, RequestAssetDto requestAssetDto);
        Task<ApiResponse> DeleteAsync(Guid id);

    }
}
