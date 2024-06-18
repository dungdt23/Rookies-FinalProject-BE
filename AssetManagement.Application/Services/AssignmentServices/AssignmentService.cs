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

			if(await _assignmentRepository.AddAsync(assignment) > 0)
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

		public Task<PagedResponse<ResponseAssetDto>> GetAllAsync(AssetFilter filter, int? index, int? size)
		{
			throw new NotImplementedException();
		}

        public Task<ApiResponse> UpdateAsync(RequestAssignmentDto request)
        {
            throw new NotImplementedException();
        }
    }
}
