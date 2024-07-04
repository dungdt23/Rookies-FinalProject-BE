using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.IRepositories
{
    public interface IReturnRequestRepository : IGenericRepository<ReturnRequest>
    {
        Task<(IEnumerable<ReturnRequest> ReturnRequests, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        ReturnRequestSortField sortField,
        TypeOrder sortOrder,
        TypeRequestState? requestState,
        DateOnly? returnedDate,
        string? search,
        Guid locationId);
    }
}
