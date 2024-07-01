using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AssetManagement.Infrastructure.Repositories;

public class ReturnRequestRepository : GenericRepository<ReturnRequest>, IReturnRequestRepository
{
    private readonly AssetManagementDBContext _dbContext;
    public ReturnRequestRepository(AssetManagementDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<(IEnumerable<ReturnRequest> ReturnRequests, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        ReturnRequestSortField sortField,
        TypeOrder sortOrder,
        TypeAssetState? assetState,
        DateOnly? returnedDate,
        string? search,
        Guid locationId)
    {
        var query = _dbContext.ReturnRequests
            .Include(rr => rr.Requestor)
            .Include(rr => rr.Responder)
            .Include(rr => rr.Assignment)
                .ThenInclude(a => a.Asset)
            .AsQueryable();


        // Ignore SoftDelete and Reject state
        query = query.Where(x => !x.IsDeleted
        && x.State != TypeRequestState.Rejected
        // Apply location filter
        && x.Assignment.Asset.LocationId == locationId);

        // Apply search
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Assignment.Asset.AssetName.Contains(search)
            || b.Assignment.Asset.AssetCode.Contains(search)
            || b.Requestor.UserName.Contains(search));
        }

        Expression<Func<ReturnRequest, object>> expressionOrder;

        // Apply sorting
        switch (sortField)
        {
            case ReturnRequestSortField.AssetCode:
                expressionOrder = e => e.Assignment.Asset.AssetCode;
                break;
            case ReturnRequestSortField.AssetName:
                expressionOrder = e => e.Assignment.Asset.AssetName;
                break;
            case ReturnRequestSortField.AssetAssignedDate:
                expressionOrder = e => e.Assignment.AssignedDate;
                break;
            case ReturnRequestSortField.RequestBy:
                expressionOrder = e => e.Requestor.UserName;
                break;
            case ReturnRequestSortField.RespondBy:
                expressionOrder = e => e.Responder.UserName;
                break;
            case ReturnRequestSortField.ReturnedDate:
                expressionOrder = e => e.ReturnedDate;
                break;
            case ReturnRequestSortField.State:
                expressionOrder = e => e.State;
                break;
            default:
                expressionOrder = e => e.CreatedAt;
                break;
        }

        if (sortOrder == TypeOrder.Descending)
        {
            query = query.OrderByDescending(expressionOrder);
        }
        else
        {
            query = query.OrderBy(expressionOrder);
        }

        //Apply filter (state & return date)

        if (assetState.HasValue)
        {
            query = query.Where(b => b.Assignment.Asset.State == assetState.Value);
        }

        if (returnedDate.HasValue)
        {
            query = query.Where(b => b.ReturnedDate != null && DateOnly.FromDateTime((DateTime)b.ReturnedDate) == returnedDate.Value);
        }

        var totalCount = await query.CountAsync();
        var books = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (books, totalCount);
    }
}
