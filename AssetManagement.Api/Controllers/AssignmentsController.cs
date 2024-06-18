using AssetManagement.Api.Authorizations;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IServices.IAssignmentServices;
using AssetManagement.Application.Services.AssignmentServices;
using AssetManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
		[Authorize(Roles = TypeNameContraint.TypeAdmin)]
		public async Task<IActionResult> Post([FromBody] RequestAssignmentDto request)
		{
			var userIdClaim = HttpContext.GetClaim("id");
			request.AssigneeId = new Guid(userIdClaim);
			var result = await _assignmentService.CreateAsync(request);
			if (result.StatusCode == StatusCodes.Status500InternalServerError)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, result);
			}

			return Ok(result);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = TypeNameContraint.TypeAdmin)]
		public async Task<IActionResult> Put(Guid id, [FromBody] RequestAssignmentDto request)
		{
			var result = await _assignmentService.UpdateAsync(id, request);
			if (result.StatusCode == StatusCodes.Status404NotFound)
			{
				return NotFound(result);
			}
			if (result.StatusCode == StatusCodes.Status500InternalServerError)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, result);
			}

			return Ok(result);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = TypeNameContraint.TypeAdmin)]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _assignmentService.DeleteAsync(id);
			if (result.StatusCode == StatusCodes.Status404NotFound)
			{
				return NotFound(result);
			}
			if (result.StatusCode == StatusCodes.Status500InternalServerError)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, result);
			}
			return Ok(result);
		}
	}
}
