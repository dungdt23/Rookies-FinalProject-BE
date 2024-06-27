using AssetManagement.Application.Dtos.ReturnRequest;

namespace AssetManagement.Application.IServices
{
    public interface IReturnRequestService
    {
        Task<(IEnumerable<ReturnRequestGetAllViewModel>, int totalCount)> GetAllReturnRequestAsync(GetAllReturnRequest request, Guid locationId);
    }
}
