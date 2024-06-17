using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
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
	}
}
