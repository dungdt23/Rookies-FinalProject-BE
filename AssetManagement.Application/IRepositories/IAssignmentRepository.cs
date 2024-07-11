using AssetManagement.Application.Filters;
using AssetManagement.Domain.Entities;
using System.Linq.Expressions;

namespace AssetManagement.Application.IRepositories;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
    IQueryable<Assignment> GetAll(bool? own,Func<Assignment, object> sortCondition, AssignmentFilter filter, Guid userId, UserType userType, Guid locationId);
    IQueryable<Assignment> GetHistoryByCondition(Expression<Func<Assignment, bool>> condition);
}
