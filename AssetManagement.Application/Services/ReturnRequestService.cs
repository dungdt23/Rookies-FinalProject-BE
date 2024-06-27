using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using AutoMapper;

namespace AssetManagement.Application.Services
{
    public class ReturnRequestService : IReturnRequestService
    {
        public readonly IReturnRequestRepository _returnRequestRepository;
        public readonly IMapper _mapper;

        public ReturnRequestService(IReturnRequestRepository returnRequestRepository, IMapper mapper)
        {
            _returnRequestRepository = returnRequestRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<ReturnRequestGetAllViewModel>, int totalCount)> GetAllReturnRequestAsync(GetAllReturnRequest request, Guid locationId)
        {
            var (returnRequests, totalCount) = await _returnRequestRepository.GetAllAsync(
                request.Page,
                request.PerPage,
                request.SortField,
                request.SortOrder,
                request.AssetState,
                request.ReturnedDate,
                request.Search,
                locationId);

            var returnRequestViewModels = _mapper.Map<IEnumerable<ReturnRequestGetAllViewModel>>(returnRequests);

            return (returnRequestViewModels, totalCount);
        }
    }
}
