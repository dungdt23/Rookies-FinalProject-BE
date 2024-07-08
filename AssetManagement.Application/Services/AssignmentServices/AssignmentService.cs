using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.IAssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services.AssignmentServices
{
	public class AssignmentService : IAssignmentService
	{
		private readonly IAssignmentRepository _assignmentRepository;
		private readonly IAssetRepository _assetRepository;
		private readonly IMapper _mapper;
		public AssignmentService(IAssignmentRepository assignmentRepository, IAssetRepository assetRepository, IMapper mapper)
		{
			_assignmentRepository = assignmentRepository;
			_assetRepository = assetRepository;
			_mapper = mapper;
		}
		public async Task<ApiResponse> CreateAsync(RequestAssignmentDto request)
		{
			var asset = await _assetRepository.GetByCondition(a => a.Id == request.AssetId && !a.IsDeleted).FirstOrDefaultAsync();
			if (asset == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageConstant.AssetNotFound,
					Data = $"Asset Id: {request.AssetId}"
				};
			}

			if (asset.State != TypeAssetState.Available)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status409Conflict,
					Message = AssignmentApiResponseMessageConstant.AssetNotAvailable,
					Data = $"Asset Id: {request.AssetId}"
				};
			}

			asset.State = TypeAssetState.Assigned;

			var assignment = _mapper.Map<Assignment>(request);

			assignment.State = TypeAssignmentState.WaitingForAcceptance;

			if (await _assignmentRepository.AddAsync(assignment) > 0 && await _assetRepository.UpdateAsync(asset) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentCreateSuccess,
					Data = _mapper.Map<ResponseAssignmentDto>(await _assignmentRepository.GetByCondition(a => a.Id == assignment.Id)
																						.Include(a => a.Assigner)
																						.Include(a => a.Assignee)
																						.Include(a => a.Asset)
																						.AsNoTracking()
																						.FirstOrDefaultAsync())
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentCreateFail,
					Data = _mapper.Map<ResponseAssignmentDto>(assignment)
				};

			}
		}

		public async Task<ApiResponse> DeleteAsync(Guid id)
		{
			var assignment = await _assignmentRepository.GetByCondition(a => a.Id == id).AsNoTracking().FirstOrDefaultAsync();

			if (assignment == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
					Data = id
				};
			}

			var asset = await _assetRepository.GetByCondition(a => a.Id == assignment.AssetId).FirstOrDefaultAsync();

			if (asset == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = AssignmentApiResponseMessageConstant.AssetNotFound,
					Data = assignment.AssetId
				};
			}

			asset.State = TypeAssetState.Available;

			if (assignment.State == TypeAssignmentState.Accepted)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = AssignmentApiResponseMessageConstant.AssignmentDeleteStateConfict,
					Data = assignment.State.ToString()
				};
			}

			if (await _assignmentRepository.DeleteAsync(id) > 0 && await _assetRepository.UpdateAsync(asset) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentDeleteSuccess,
					Data = _mapper.Map<ResponseAssignmentDto>(assignment)
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentDeleteFail,
					Data = _mapper.Map<ResponseAssignmentDto>(assignment)
				};
			}
		}

		public async Task<PagedResponse<ResponseAssignmentDto>> GetAllAsync(bool? own, AssignmentFilter filter, Guid userId, UserType userType, Guid locationId, int? index, int? size)
		{
			Func<Assignment, object> sortCondition = x => x.Asset.AssetCode;
			switch (filter.FieldFilter)
			{
				case FieldAssignmentType.AssetName:
					sortCondition = x => x.Asset.AssetName;
					break;
				case FieldAssignmentType.AssignedTo:
					sortCondition = x => x.Assignee.UserName;
					break;
				case FieldAssignmentType.AssignedBy:
					sortCondition = x => x.Assigner.UserName;
					break;
				case FieldAssignmentType.AssignedDate:
					sortCondition = x => x.AssignedDate;
					break;
				case FieldAssignmentType.State:
					sortCondition = x => x.State;
					break;
			}
			var assignmentsQuery = _assignmentRepository.GetAll(own, sortCondition, filter, userId, userType, locationId);
			var totalCount = assignmentsQuery.Count();

			if (totalCount == 0)
			{
				return new PagedResponse<ResponseAssignmentDto>
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentGetNotFound,
					Data = new List<ResponseAssignmentDto>()
				};

			}



			var assignments = assignmentsQuery.Skip((index.Value - 1) * size.Value).Take(size.Value).ToList();

			var assignmentDtos = _mapper.Map<List<ResponseAssignmentDto>>(assignments);
			return new PagedResponse<ResponseAssignmentDto>
			{
				StatusCode = StatusCodes.Status200OK,
				Data = assignmentDtos,
				TotalCount = totalCount,
				Message = AssignmentApiResponseMessageConstant.AssignmentGetSuccess
			};
		}

		public async Task<ApiResponse> GetByIdAsync(Guid id)
		{
			var assignment = await _assignmentRepository.GetByCondition(a => a.Id == id && !a.IsDeleted)
														.Include(a => a.Assigner)
														.Include(a => a.Assignee)
														.Include(a => a.Asset)
														.FirstOrDefaultAsync();
			if (assignment == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
					Data = id
				};
			}

			var assignmentDto = _mapper.Map<ResponseAssignmentDto>(assignment);
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status200OK,
				Message = AssignmentApiResponseMessageConstant.AssignmentGetSuccess,
				Data = assignmentDto
			};
		}

		public async Task<ApiResponse> RespondAsync(RequestAssignmentRespondDto request)
		{
			var assignment = await _assignmentRepository.GetByCondition(a => a.Id == request.AssignmentId && !a.IsDeleted)
														.Include(a => a.Assigner)
														.Include(a => a.Assignee)
														.Include(a => a.Asset)
														.FirstOrDefaultAsync();

			if (assignment == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
					Data = $"Assignment Id : {request.AssignmentId}"
				};
			}

			if (assignment.State != TypeAssignmentState.WaitingForAcceptance)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = AssignmentApiResponseMessageConstant.AssignmentRespondNotWaitingForAcceptance,
					Data = assignment.State.ToString()
				};
			}

			assignment.State = request.IsAccept ? TypeAssignmentState.Accepted : TypeAssignmentState.Declined;
			assignment.Asset.State = request.IsAccept ? TypeAssetState.Assigned : TypeAssetState.Available;

			if (await _assignmentRepository.UpdateAsync(assignment) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentRespondSuccess,
					Data = _mapper.Map<ResponseAssignmentDto>(assignment)
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentRespondFail,
					Data = _mapper.Map<ResponseAssignmentDto>(assignment)
				};


			}
		}

		public async Task<ApiResponse> UpdateAsync(Guid id, RequestAssignmentDto request)
		{
			var assignment = await _assignmentRepository.GetByCondition(a => a.Id == id && !a.IsDeleted)
														.FirstOrDefaultAsync();
			if (assignment == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
					Data = id
				};
			}

			if (request.AssetId != assignment.AssetId)
			{
				var asset = await _assetRepository.GetByCondition(a => a.Id == assignment.AssetId).FirstOrDefaultAsync();

				asset.State = TypeAssetState.Available;

				if (await _assetRepository.UpdateAsync(asset) == 0)
				{
					return new ApiResponse
					{
						StatusCode = StatusCodes.Status500InternalServerError,
						Message = AssignmentApiResponseMessageConstant.AssignmentUpdateFail,
						Data = _mapper.Map<ResponseAssignmentDto>(assignment)
					};

				}
			}



			request.AssignerId = assignment.AssignerId;

			_mapper.Map(request, assignment);

			var newAsset = await _assetRepository.GetByCondition(a => a.Id == assignment.AssetId).FirstOrDefaultAsync();

			newAsset.State = TypeAssetState.Assigned;

			assignment.Asset = newAsset;

			if (await _assignmentRepository.UpdateAsync(assignment) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentUpdateSuccess,
					Data = _mapper.Map<ResponseAssignmentDto>(assignment)
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentUpdateFail,
					Data = _mapper.Map<ResponseAssignmentDto>(assignment)
				};

			}
		}
		public async Task<PagedResponse<ResponseHistoryAsmDto>> GetByAssetIdAsync(Guid assetId, int index, int size)
		{
			var historicalAsm = await _assignmentRepository.GetByCondition(x => x.AssetId == assetId)
				.Include(x => x.Asset)
				.Include(x => x.Assigner)
				.Include(x => x.Assignee)
				.Include(x => x.ReturnRequests)
				.OrderByDescending(x => x.CreatedAt)
				.Skip((index - 1) * size).Take(size)
				.ToListAsync();
			var historicalAsmDto = _mapper.Map<IEnumerable<ResponseHistoryAsmDto>>(historicalAsm);
			return new PagedResponse<ResponseHistoryAsmDto>
			{
				Data = historicalAsmDto,
				TotalCount = historicalAsm.Count()
			};
		}
	}
}
