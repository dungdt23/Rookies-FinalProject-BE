using AssetManagement.Application.Filters;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IRepositories;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
    Task<IEnumerable<Asset>> GetAllAsync(Func<Asset, object> sortCondition, AssignmentFilter filter, int? index, int? size);
    Task<int> GetTotalCountAsync(Assignment filter);
}
