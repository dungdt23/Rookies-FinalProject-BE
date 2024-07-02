﻿using AssetManagement.Application.Dtos.ReturnRequest;

namespace AssetManagement.Application.IServices
{
    public interface IReturnRequestService
    {
        Task<(IEnumerable<ReturnRequestGetAllViewModel>, int totalCount)> GetAllReturnRequestAsync(
            GetAllReturnRequest request,
            Guid userId);
        Task<ReturnRequestViewModel> CreateReturnRequestAsync(
            CreateReturnRequestRequest request,
            Guid userId);
        Task CompleteReturnRequestAsync(
            Guid returnRequestId,
            Guid requesterId);

        Task RejectReturnRequestAsync(
            Guid returnRequestId,
            Guid requesterId);
    }
}
