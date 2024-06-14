using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IServices.IUserServices;

public interface IUserService
{
    Task<ApiResponse> CreateAsync(CreateUserForm form);
    User EncryptPassword(User user, string password);

    bool CheckPassword(User user, string password);
    Task<PagedResponse<ResponseUserDto>> GetAllAsync(UserFilter filter, int? index, int? size);
}
