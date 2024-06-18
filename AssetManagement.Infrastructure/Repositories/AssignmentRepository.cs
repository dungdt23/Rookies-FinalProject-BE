using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Entities;
using AssetManagement.Infrastructure.Migrations;

namespace AssetManagement.Infrastructure.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    private readonly AssetManagementDBContext _dbContext;
    public AssignmentRepository(AssetManagementDBContext dBContext) : base(dBContext)
    {
        _dbContext = dBContext;
    }
    private IQueryable<Assignment> ApplyFilter(AssignmentFilter filter){
        IQueryable<Assignment> query = _dbContext.Assignments.Where(x => !x.IsDeleted && x.Assigner.LocationId == filter.LocationId);
        if (filter.UserType.HasValue)
        {
            if(filter.UserType.Value == UserType.Staff){
                query = query.Where(x => x.AssigneeId == filter.userId);
            }
        }
        var searchString = filter.SearchString;

        query = query.Where(x =>
        (string.IsNullOrEmpty(searchString) || (!string.IsNullOrEmpty(searchString)
        && (x.Asset.AssetCode.Contains(searchString) || x.Asset.AssetName.Contains(searchString) || x.Assignee.UserName.Contains(searchString)))));
        return query;
    }
    public Task<IEnumerable<Asset>> GetAllAsync(Func<Asset, object> sortCondition, AssignmentFilter filter, int? index, int? size)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalCountAsync(Assignment filter)
    {
        throw new NotImplementedException();
    }

}
