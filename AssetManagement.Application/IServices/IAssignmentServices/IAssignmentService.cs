using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.IServices.IAssignmentServices
{
	public interface IAssignmentService
	{
		Task<ApiResponse> CreateAsync(RequestAssignmentDto request);
		Task<PagedResponse<ResponseAssetDto>> GetAllAsync(AssignmentFilter filter, int? index, int? size);
		Task<ApiResponse> UpdateAsync(Guid id,RequestAssignmentDto request);
		Task<ApiResponse> DeleteAsync(Guid id);


	}
}
