using AssetManagement.Application.Filters;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    string GenerateStaffCode();
    string GenerateUserName(string fullName);
    Task<IEnumerable<User>> GetAllAsync(Func<User, object> condition,UserFilter filter, int? index, int? size);
    Task<int> GetTotalCountAsync(UserFilter filter);


}
