﻿using AssetManagement.Api.Authorizations;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
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
			Guid userIdGuild;
			if (!Guid.TryParse(userIdClaim, out userIdGuild))
			{
				return Unauthorized();
			}
			request.AssignerId = userIdGuild;
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

		[HttpPut("respond")]
		[Authorize]
		public async Task<IActionResult> Respond([FromBody] RequestAssignmentRespondDto request)
		{
			var result = await _assignmentService.RespondAsync(request);
			if (result.StatusCode == StatusCodes.Status404NotFound)
			{
				return NotFound(result);
			}
			if (result.StatusCode == StatusCodes.Status400BadRequest)
			{
				return BadRequest(result);
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
			if (result.StatusCode == StatusCodes.Status400BadRequest)
			{
				return BadRequest(result);
			}
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

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Get([FromQuery] AssignmentFilter filter, int index = 1, int size = 10)
		{
			var locationIdClaim = HttpContext.GetClaim("locationId");
			Guid locationIdGuid;
			if (!Guid.TryParse(locationIdClaim, out locationIdGuid))
			{
				return Unauthorized();
			}
			var userIdClaim = HttpContext.GetClaim("id");
			Guid userIdGuid;
			if (!Guid.TryParse(userIdClaim, out userIdGuid))
			{
				return Unauthorized();
			}
			var role = HttpContext.GetClaim("role");
			object? roleEnum;
			if (!Enum.TryParse(typeof(UserType), role, out roleEnum))
			{
				return Unauthorized();
			}
			filter.UserId = userIdGuid;
			filter.UserType = (UserType) roleEnum;
			filter.LocationId = locationIdGuid;
			var result = await _assignmentService.GetAllAsync(filter, index, size);
			if (result.StatusCode == StatusCodes.Status404NotFound)
			{
				return NotFound(result);
			}
			return Ok(result);
		}

		[HttpGet("{id:guid}")]
		[Authorize(Roles = TypeNameContraint.TypeAdmin)]
		public async Task<IActionResult> Get(Guid id)
		{
			var result = await _assignmentService.GetByIdAsync(id);
			if (result.StatusCode == StatusCodes.Status404NotFound)
			{
				return NotFound(result);
			}
			return Ok(result);

		}
	}
}
