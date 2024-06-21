using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Entities;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    private readonly AssetManagementDBContext _dbContext;
    public AssignmentRepository(AssetManagementDBContext dBContext) : base(dBContext)
    {
        _dbContext = dBContext;
    }
    private IQueryable<Assignment> ApplyFilter(AssignmentFilter filter)
    {
        IQueryable<Assignment> query = _dbContext.Assignments.Where(x => !x.IsDeleted);
        if (filter.UserType == UserType.Staff && filter.UserId.HasValue)
        {
            query = query.Where(x => x.AssigneeId == filter.UserId.Value);
        }

        if (filter.StateFilter.HasValue)
        {
            query = query.Where(x => x.State == filter.StateFilter.Value);
        }

        if (filter.AssignedDateFilter.HasValue)
        {
            query = query.Where(x => x.AssignedDate.Date == filter.AssignedDateFilter.Value);
        }
        var searchString = filter.SearchString;

        query = query.Where(x =>
        string.IsNullOrEmpty(searchString) || (!string.IsNullOrEmpty(searchString)
        && (x.Asset.AssetCode.Contains(searchString) || x.Asset.AssetName.Contains(searchString) || x.Assignee.UserName.Contains(searchString))));
        return query;
    }
    public IQueryable<Assignment> GetAll(Func<Assignment, object> sortCondition, AssignmentFilter filter)
    {
        var query = ApplyFilter(filter);
        var assignments = query.Include(a => a.Assigner)
                                .Include(a => a.Assignee)
                                .Include(a => a.Asset)
                                .AsNoTracking();
        if (filter.IsAscending)
        {
            assignments = assignments.OrderBy(sortCondition).AsQueryable();
        }
        else
        {
            assignments = assignments.OrderByDescending(sortCondition).AsQueryable();
        }

        return assignments;
    }

}
