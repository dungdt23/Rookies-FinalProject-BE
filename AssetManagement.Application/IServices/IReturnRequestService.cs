using AssetManagement.Application.Dtos.ReturnRequest;

namespace AssetManagement.Application.IServices
{
    public interface IReturnRequestService
    {
        Task<(IEnumerable<ReturnRequestGetAllViewModel>, int totalCount)> GetAllReturnRequestAsync(
            GetAllReturnRequest request,
            Guid locationId);
        Task<ReturnRequestViewModel> CreateReturnRequestAsync(
            CreateReturnRequestRequest request,
            Guid requesterId);
        Task CompleteReturnRequestAsync(
            Guid returnRequestId,
            Guid requesterId);

        Task RejectReturnRequestAsync(
            Guid returnRequestId,
            Guid requesterId);
    }
}
