using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;

namespace AssetManagement.Application.IServices
{
    public interface IReturnRequestService
    {
        Task<(IEnumerable<ResponseReturnRequestGetAllDto>, int totalCount)> GetAllReturnRequestAsync(
            RequestGetAllReturnRequestDto request,
            Guid requestorId);
        Task<ResponseReturnRequestDto> CreateReturnRequestAsync(
            RequestCreateReturnRequestDto request,
            Guid userId);
        Task CompleteReturnRequestAsync(
            Guid returnRequestId,
            Guid requesterId);

        Task RejectReturnRequestAsync(
            Guid returnRequestId,
            Guid requesterId);
    }
}
