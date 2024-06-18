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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Services.AssignmentServices
{
	public class AssignmentService : IAssignmentService
	{
		private readonly IAssignmentRepository _assignmentRepository;
		private readonly IMapper _mapper;
		public AssignmentService(IAssignmentRepository assignmentRepository, IMapper mapper)
		{
			_assignmentRepository = assignmentRepository;
			_mapper = mapper;
		}
		public async Task<ApiResponse> CreateAsync(RequestAssignmentDto request)
		{
			var assignment = _mapper.Map<Assignment>(request);

			assignment.State = TypeAssignmentState.WaitingForAcceptance;

			if (await _assignmentRepository.AddAsync(assignment) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentCreateSuccess,
					Data = assignment
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentCreateFail,
					Data = assignment
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
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
					Data = id
				};
			}

			if (assignment.State != TypeAssignmentState.WaitingForAcceptance)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = AssignmentApiResponseMessageConstant.AssignmentDeleteNotWaitingForAcceptance,
					Data = assignment.State.ToString()
				};
			}

			if (await _assignmentRepository.DeleteAsync(id) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentDeleteSuccess,
					Data = assignment
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentDeleteFail,
					Data = assignment
				};
			}
		}

		public async Task<PagedResponse<ResponseAssignmentDto>> GetAllAsync(AssignmentFilter filter, int? index, int? size)
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
			var assignmentsQuery = _assignmentRepository.GetAll(sortCondition, filter);
			var totalCount = assignmentsQuery.Count();

			if (totalCount == 0)
			{
				return new PagedResponse<ResponseAssignmentDto>
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageConstant.AssignmentGetNotFound
				};

			}

			if (index.HasValue && size.HasValue)
			{
				assignmentsQuery.Skip((index.Value - 1) * size.Value).Take(size.Value);
			}
			var assignments = assignmentsQuery.ToList();
			var assignmentDtos = _mapper.Map<List<ResponseAssignmentDto>>(assignments);
			return new PagedResponse<ResponseAssignmentDto>
			{
				Data = assignmentDtos,
				TotalCount = totalCount,
				Message = AssignmentApiResponseMessageConstant.AssignmentGetSuccess
			};
		}

		public async Task<ApiResponse> RespondAsync(RequestAssignmentRespondDto request)
		{
			var assignment = await _assignmentRepository.GetByCondition(a => a.Id == request.AssignmentId)
														.Include(a => a.Asset)
														.FirstOrDefaultAsync();

			if (assignment == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
					Data = $"Request Id : {request.AssignmentId}"
				};
			}

			assignment.State = request.IsAccept ? TypeAssignmentState.Accepted : TypeAssignmentState.Rejected;
			assignment.Asset.State = request.IsAccept ? TypeAssetState.Assigned : TypeAssetState.Available;

			if (await _assignmentRepository.UpdateAsync(assignment) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentRespondSuccess,
					Data = assignment
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentRespondFail,
					Data = assignment
				};


			}
		}

		public async Task<ApiResponse> UpdateAsync(Guid id, RequestAssignmentDto request)
		{
			var assignment = await _assignmentRepository.GetByCondition(a => a.Id == id).FirstOrDefaultAsync();
			if (assignment == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
					Data = id
				};
			}
			request.AssignerId = assignment.AssignerId;
			_mapper.Map(request, assignment);

			if (await _assignmentRepository.UpdateAsync(assignment) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageConstant.AssignmentUpdateSuccess,
					Data = assignment
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageConstant.AssignmentUpdateFail,
					Data = assignment
				};

			}
		}
	}
}
