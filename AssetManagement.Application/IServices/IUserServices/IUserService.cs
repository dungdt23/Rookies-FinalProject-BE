using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IServices.IUserServices;

public interface IUserService
{
    Task<ApiResponse> LoginAsync(RequestLoginDto form, byte[] key);
    Task<ApiResponse> CreateAsync(RequestUserCreateDto form);
    Task<ApiResponse> UpdateAsync(Guid id, RequestUserEditDto form);
    User EncryptPassword(User user, string password);
    bool CheckPassword(User user, string password);
    Task<PagedResponse<ResponseUserDto>> GetAllAsync(Guid locationId, UserFilter filter, int? index, int? size);
    Task<ApiResponse> DisableUser(Guid id);
    Task<ApiResponse> GetById(Guid id);
}
