using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.Exceptions.Assignment;
using AssetManagement.Application.Exceptions.Common;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class ReturnRequestService : IReturnRequestService
    {
        public readonly IReturnRequestRepository _returnRequestRepository;
        public readonly IAssignmentRepository _assignmentRepository;
        public readonly IMapper _mapper;
        public readonly ITransactionRepository _transactionRepository;
        public readonly IUserRepository _userRepository;

        public ReturnRequestService(
            IReturnRequestRepository returnRequestRepository,
            IAssignmentRepository assignmentRepository,
            IMapper mapper,
            ITransactionRepository transactionRepository,
            IUserRepository userRepository)
        {
            _returnRequestRepository = returnRequestRepository;
            _mapper = mapper;
            _assignmentRepository = assignmentRepository;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public async Task<(IEnumerable<ReturnRequestGetAllViewModel>, int totalCount)> GetAllReturnRequestAsync(
            GetAllReturnRequest request,
            Guid userId)
        {
            var user = await _userRepository.GetByCondition(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

            var (returnRequests, totalCount) = await _returnRequestRepository.GetAllAsync(
                request.Page,
                request.PerPage,
                request.SortField,
                request.SortOrder,
                request.AssetState,
                request.ReturnedDate,
                request.Search,
                user.LocationId);

            var returnRequestViewModels = _mapper.Map<List<ReturnRequestGetAllViewModel>>(returnRequests);

            return (returnRequestViewModels, totalCount);
        }

        public async Task<ReturnRequestViewModel> CreateReturnRequestAsync(
            CreateReturnRequestRequest request,
            Guid requesterId)
        {
            var assignment = await _assignmentRepository.GetByCondition(a => a.Id == request.AssignmentId)
                .Include(a => a.Asset)
                .FirstOrDefaultAsync();
            var user = await _userRepository.GetByCondition(u => u.Id == requesterId)
                .Include(u => u.Type)
                .FirstOrDefaultAsync();


            // Validation for admin and staff
            if (user == null)
                throw new NotFoundException($"User with ID {requesterId} not found.");

            // Admin can not create return requests for assignments that have the asset's locaiton different from the admin's
            if (assignment == null)
                throw new NotFoundException($"Assignment with id {request.AssignmentId} not found.");

            if (assignment.Asset.LocationId != user.LocationId)
                throw new WrongLocationException($"You do not have access to this assignment.");

            if (user.Type.TypeName == TypeNameConstants.TypeStaff && assignment.AssigneeId != user.Id)
                throw new UnauthorizedAssignmentAccessException("You do not have access to this assignment.");

            if (assignment.State != TypeAssignmentState.Accepted)
                throw new AssignmentNotAcceptedException("Assignment must be in an accepted state.");

            if (assignment.ActiveReturnRequestId != null)
                throw new ActiveReturnRequestAlreadyExistsException($"This assignment already has an active return request.");


            ReturnRequest newReturnRequest = new ReturnRequest
            {
                RequestorId = requesterId,
                AssignmentId = request.AssignmentId,
                RequestedDate = DateTime.Now,
                State = TypeRequestState.WaitingForReturning,
            };
            assignment.ActiveReturnRequestId = newReturnRequest.Id;
            assignment.ActiveReturnRequest = newReturnRequest;

            await _transactionRepository.BeginTransactionAsync();

            try
            {
                await _returnRequestRepository.AddAsync(newReturnRequest);
                await _assignmentRepository.UpdateAsync(assignment);
                await _transactionRepository.CommitTransactionAsync();

            }
            catch (Exception)
            {
                await _transactionRepository.RollbackTransactionAsync();
                throw;
            }

            return _mapper.Map<ReturnRequestViewModel>(newReturnRequest);
        }
    }
}
