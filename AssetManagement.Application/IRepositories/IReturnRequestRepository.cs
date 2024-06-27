using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.IRepositories
{
    public interface IReturnRequestRepository
    {
        Task<(IEnumerable<ReturnRequest> ReturnRequests, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        ReturnRequestSortField sortField,
        TypeOrder sortOrder,
        TypeAssetState? assetState,
        DateOnly? returnedDate,
        string? search,
        Guid locationId);
    }
}
