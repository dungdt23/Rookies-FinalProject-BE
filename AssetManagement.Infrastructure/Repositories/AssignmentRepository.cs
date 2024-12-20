using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Entities;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AssetManagement.Infrastructure.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository   
{
	private readonly AssetManagementDBContext _dbContext;
	public AssignmentRepository(AssetManagementDBContext dBContext) : base(dBContext)
	{
		_dbContext = dBContext;
	}
	private IQueryable<Assignment> ApplyFilter(bool? own, AssignmentFilter filter, Guid userId, UserType userType, Guid locationId)
	{
		IQueryable<Assignment> query = _dbContext.Assignments.Where(x => !x.IsDeleted && x.Assigner.LocationId == locationId);
		if (userType == UserType.Staff)
		{
			query = query.Where(x => x.AssigneeId == userId && x.State != Domain.Enums.TypeAssignmentState.Declined && x.AssignedDate.Date <= DateTime.UtcNow.Date);
		}
		else if (own.HasValue && own.Value)
		{
			query = query.Where(x => x.AssigneeId == userId && x.State != Domain.Enums.TypeAssignmentState.Declined && x.AssignedDate.Date <= DateTime.UtcNow.Date);
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
	public IQueryable<Assignment> GetAll(bool? own, Func<Assignment, object> sortCondition, AssignmentFilter filter, Guid userId, UserType userType, Guid locationId)
	{
		var query = ApplyFilter(own, filter, userId, userType, locationId);
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

    public IQueryable<Assignment> GetHistoryByCondition(Expression<Func<Assignment, bool>> condition)
    {
        return _dbContext.Assignments.Where(condition);
    }
}
