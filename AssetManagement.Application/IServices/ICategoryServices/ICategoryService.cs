using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;

namespace AssetManagement.Application.IServices.ICategoryServices
{
    public interface ICategoryService
    {
        Task<ApiResponse> GetAllAsync(int? index, int? size);

    }
}
