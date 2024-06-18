using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IServices.IAssignmentServices;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [Route("assignments")]
	[ApiController]
	public class AssignmentsController : ControllerBase
	{
		private readonly IAssignmentService _assignmentService;
		public AssignmentsController(IAssignmentService assignmentService)
		{
			_assignmentService = assignmentService;
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] RequestAssignmentDto request)
		{
			var result = await _assignmentService.CreateAsync(request);
			if (result.StatusCode == StatusCodes.Status500InternalServerError)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, result);
			}

			return Ok(result);
		}
	}
}
