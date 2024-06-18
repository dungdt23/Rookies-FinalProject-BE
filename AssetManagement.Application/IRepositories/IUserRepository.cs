using AssetManagement.Application.Filters;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    string GenerateStaffCode();
    string GenerateUserName(string fullName);
    Task<IEnumerable<User>> GetAllAsync(Func<User, object> condition,Guid locationId,UserFilter filter, int? index, int? size);
    Task<int> GetTotalCountAsync(Guid locationId, UserFilter filter);


}
