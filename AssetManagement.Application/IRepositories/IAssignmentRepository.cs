using AssetManagement.Application.Filters;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IRepositories;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
    IQueryable<Assignment> GetAll(Func<Assignment, object> sortCondition, AssignmentFilter filter);
}
