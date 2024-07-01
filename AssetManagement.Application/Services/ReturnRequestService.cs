using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.Exceptions.Assignment;
using AssetManagement.Application.Exceptions.Common;
using AssetManagement.Application.Exceptions.ReturnRequest;
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
        private readonly IReturnRequestRepository _returnRequestRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;


        public ReturnRequestService(
            IReturnRequestRepository returnRequestRepository,
            IAssignmentRepository assignmentRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository)
        {
            _returnRequestRepository = returnRequestRepository;
            _mapper = mapper;
            _assignmentRepository = assignmentRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
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

            if (assignment == null)
                throw new NotFoundException($"Assignment with id {request.AssignmentId} not found.");

            // Admin can not create return requests for assignments that have the asset's locaiton different from the admin's
            if (assignment.Asset.LocationId != user.LocationId)
                throw new WrongLocationException($"You do not have access to this assignment.");

            // Staff cannot create return requests for assignments that is not their's
            if (user.Type.TypeName == TypeNameConstants.TypeStaff && assignment.AssigneeId != user.Id)
                throw new UnauthorizedAssignmentAccessException("You do not have access to this assignment.");

            if (assignment.State != TypeAssignmentState.Accepted)
                throw new AssignmentNotAcceptedException("Assignment must be in an accepted state.");

            if (assignment.ActiveReturnRequestId != null)
                throw new ActiveReturnRequestAlreadyExistsException($"This assignment already has an active return request.");


            var newReturnRequest = new ReturnRequest
            {
                RequestorId = requesterId,
                AssignmentId = request.AssignmentId,
                RequestedDate = DateTime.Now,
                State = TypeRequestState.WaitingForReturning,
                LocationId = assignment.Asset.LocationId,
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
            catch (Exception ex)
            {
                await _transactionRepository.RollbackTransactionAsync();
                throw;
            }

            return _mapper.Map<ReturnRequestViewModel>(newReturnRequest);
        }

        public async Task CompleteReturnRequestAsync(Guid returnRequestId, Guid requesterId)
        {
            var user = await _userRepository.GetByCondition(u => u.Id == requesterId)
                .FirstOrDefaultAsync();

            var returnRequest = await _returnRequestRepository.GetByCondition(rr => rr.Id == returnRequestId)
                .Include(rr => rr.Assignment)
                    .ThenInclude(a => a.Asset)
                .FirstOrDefaultAsync();

            if (returnRequest == null)
                throw new NotFoundException($"Return Request with id {returnRequestId} not found.");

            if (returnRequest.LocationId != user.LocationId)
                throw new WrongLocationException($"You do not have access to this assignment.");

            if (returnRequest.State != TypeRequestState.WaitingForReturning)
                throw new ReturnRequestNotWaitingException("Return Request must be in a waiting state.");

            returnRequest.State = TypeRequestState.Completed;
            returnRequest.ReturnedDate = DateTime.Now;
            returnRequest.ResponderId = user.Id;
            returnRequest.Responder = user;
            returnRequest.Assignment.IsDeleted = true;
            returnRequest.Assignment.DeletedAt = DateTime.Now;
            returnRequest.Assignment.Asset.State = TypeAssetState.Available;

            await _transactionRepository.BeginTransactionAsync();
            try
            {
                await _returnRequestRepository.UpdateAsync(returnRequest);
                await _transactionRepository.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _transactionRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RejectReturnRequestAsync(Guid returnRequestId, Guid requesterId)
        {
            var returnRequest = await _returnRequestRepository.GetByCondition(rr => rr.Id == returnRequestId)
                .Include(rr => rr.Assignment)
                    .ThenInclude(a => a.Asset)
                .FirstOrDefaultAsync();
            var user = await _userRepository.GetByCondition(u => u.Id == requesterId)
                .FirstOrDefaultAsync();

            if (returnRequest == null)
                throw new NotFoundException($"Return Request with id {returnRequestId} not found.");

            if (returnRequest.LocationId != user.LocationId)
                throw new WrongLocationException($"You do not have access to this assignment.");

            if (returnRequest.State != TypeRequestState.WaitingForReturning)
                throw new ReturnRequestNotWaitingException("Return Request must be in a waiting state.");

            returnRequest.State = TypeRequestState.Rejected;
            returnRequest.ReturnedDate = DateTime.Now;
            returnRequest.ResponderId = user.Id;
            returnRequest.Responder = user;
            returnRequest.Assignment.ActiveReturnRequestId = null;
            returnRequest.Assignment.ActiveReturnRequest = null;

            await _transactionRepository.BeginTransactionAsync();
            try
            {
                await _returnRequestRepository.UpdateAsync(returnRequest);
                await _transactionRepository.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _transactionRepository.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
