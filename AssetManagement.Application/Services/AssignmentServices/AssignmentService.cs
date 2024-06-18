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
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Services.AssignmentServices
{
	public class AssignmentService : IAssignmentService
	{
		private readonly IGenericRepository<Assignment> _assignmentRepository;
		private readonly IMapper _mapper;
		public AssignmentService(IGenericRepository<Assignment> assignmentRepository, IMapper mapper)
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
					Message = AssignmentApiResponseMessageContraint.AssignmentCreateSuccess,
					Data = assignment
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageContraint.AssignmentCreateFail,
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
					Message = AssignmentApiResponseMessageContraint.AssignmentNotFound,
					Data = id
				};
			}

			if (assignment.State != TypeAssignmentState.WaitingForAcceptance)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = AssignmentApiResponseMessageContraint.AssignmentDeleteNotWaitingForAcceptance,
					Data = assignment.State.ToString()
				};
			}

			if (await _assignmentRepository.DeleteAsync(id) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageContraint.AssignmentDeleteSuccess,
					Data = assignment
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageContraint.AssignmentDeleteFail,
					Data = assignment
				};
			}
		}

        public Task<PagedResponse<ResponseAssetDto>> GetAllAsync(AssignmentFilter filter, int? index, int? size)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> UpdateAsync(Guid id, RequestAssignmentDto request)
		{
			var assignment = await _assignmentRepository.GetByCondition(a => a.Id == id).FirstOrDefaultAsync();
			if (assignment == null)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = AssignmentApiResponseMessageContraint.AssignmentNotFound,
					Data = id
				};
			}
			request.AssigneeId = assignment.AssigneeId;
			_mapper.Map(request, assignment);

			if (await _assignmentRepository.UpdateAsync(assignment) > 0)
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = AssignmentApiResponseMessageContraint.AssignmentCreateSuccess,
					Data = assignment
				};
			}
			else
			{
				return new ApiResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					Message = AssignmentApiResponseMessageContraint.AssignmentCreateFail,
					Data = assignment
				};

			}
		}
	}
}
